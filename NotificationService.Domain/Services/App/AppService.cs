using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Data.Entity;
using NotificationService.Data.Enums;
using NotificationService.Domain.Contracts.Interfaces;
using NotificationService.Domain.Contracts.Models.DomainModels;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Contracts.Models.Result;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Domain.Services.App;

public class AppService : IAppService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private const string UniqueEmailError = "Email should be unique";
    private const string NotificationCouldNotBeFoundError = "Notification could not be found";
    private const string UserCouldNotBeFoundError = "User could not be found";
    private const string UserConfigurationCouldNotBeFoundError = "User configurations could not be found";
    private const string NotificationStatusError = "Unable to process - notification is unavailable to send.";
    private const string NewNotificatonMessage = "Notification was created";
    private const string UpdateNotificatonMessage = "Notification was updated";
    private const string NotificationIsCancelled = "Notification is already cancelled";

    public AppService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateUser(CreateUserRequestModel userModel)
    {
        var user = await _context.Users.Where(x => x.Email == userModel.Email).ToListAsync();
        if (user.Count != 0)
        {
            throw new DuplicateEmailException(UniqueEmailError);
        }

        _context.Users.Add(_mapper.Map<CreateUserRequestModel, User>(userModel));
        await _context.SaveChangesAsync();
    }

    public async Task CreateNotification(CreateNotificationRequestModel notificationModel)
    {
        var notification = _mapper.Map<CreateNotificationRequestModel, Notification>(notificationModel);
        var sender = await _context.UserConfigs.FirstOrDefaultAsync(x =>
            x.UserId == notification.SenderId && x.Channel == notification.Channel);

        if (sender == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        var receiver = await _context.Users.FirstOrDefaultAsync(x => x.Id == notification.ReceiverId);

        if (receiver == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        var statusModel = new NotificationStatusModel
        {
            Id = Guid.NewGuid().ToString(),
            NotificationId = notification.Id,
            Status = StatusType.New,
            TimeStamp = DateTime.Now,
            Message = NewNotificatonMessage
        };

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                await _context.NotificationStatuses.AddAsync(
                    _mapper.Map<NotificationStatusModel, NotificationStatus>(statusModel));
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new DbUpdateException(ex.Message);
            }
        }
    }

    public async Task<List<NotificationResponseModel>> GetNotifications()
    {
        var notifications = await _context.Notifications.ToListAsync();

        var latestStatuses = await _context.NotificationStatuses
            .GroupBy(s => s.NotificationId)
            .Select(g => g.OrderByDescending(s => s.TimeStamp).FirstOrDefault())
            .ToListAsync();

        var notificationsWithLatestStatus = notifications
            .Join(
                latestStatuses,
                notification => notification.Id,
                statusModel => statusModel.NotificationId,
                (notification, statusModel) =>
                {
                    var responseModel = _mapper.Map<Notification, NotificationResponseModel>(notification);
                    responseModel.Status = statusModel.Status;
                    responseModel.TimeStamp = statusModel.TimeStamp;
                    responseModel.Message = statusModel.Message;

                    return responseModel;
                })
            .ToList();


        return notificationsWithLatestStatus;
    }

    public async Task<List<UserModel>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        var mappedUsers = _mapper.Map<List<User>, List<UserModel>>(users);

        return mappedUsers;
    }

    public async Task<UserModel> GetUserById(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        return _mapper.Map<User, UserModel>(user);
    }

    public async Task AddUserConfig(UserConfigModel userConfig)
    {
        userConfig.Token = EncryptionService.Encrypt(userConfig.Token);
        _context.UserConfigs.Add(_mapper.Map<UserConfigModel, UserConfig>(userConfig));
        await _context.SaveChangesAsync();
    }

    public async Task<NotificationResponseModel> GetNotificationById(string id)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);

        if (notification == null)
        {
            throw new NotFoundException(NotificationCouldNotBeFoundError);
        }

        var latestStatus = await _context.NotificationStatuses
            .Where(s => s.NotificationId == notification.Id)
            .OrderByDescending(s => s.TimeStamp)
            .FirstOrDefaultAsync();

        if (latestStatus == null)
        {
            throw new InvalidDataException("Notification should has a status");
        }

        var responseNotification = _mapper.Map<Notification, NotificationResponseModel>(notification);
        responseNotification.Status = latestStatus.Status;
        responseNotification.TimeStamp = latestStatus.TimeStamp;
        responseNotification.Message = latestStatus.Message;

        return responseNotification;
    }

    public async Task<SendNotificationModel> GetNotificationDataForSend(string id)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
        if (notification == null)
        {
            throw new NotFoundException(NotificationCouldNotBeFoundError);
        }

        var notificationStatus =
            await _context.NotificationStatuses.Where(x =>
                    x.NotificationId == id && (x.Status == StatusType.Cancelled || x.Status == StatusType.Success))
                .FirstOrDefaultAsync();
        if (notificationStatus != null)
        {
            throw new NotificationException(NotificationStatusError);
        }


        var senderModel = await (from user in _context.Users
            join userConfig in _context.UserConfigs on user.Id equals userConfig.UserId
            where userConfig.Channel == notification.Channel && user.Id == notification.SenderId
            select new SenderResponseModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = userConfig.Token
            }).FirstOrDefaultAsync();

        if (senderModel == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        var receiverModel = await _context.Users.FirstOrDefaultAsync(x => x.Id == notification.ReceiverId);
        if (receiverModel == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        var model = new SendNotificationModel
        {
            Notification = _mapper.Map<Notification, NotificationModel>(notification),
            Sender = senderModel,
            Receiver = _mapper.Map<User, UserModel>(receiverModel)
        };

        return model;
    }

    public async Task UpdateNotification(NotificationModel notification)
    {
        var sender = await _context.UserConfigs.FirstOrDefaultAsync(x =>
            x.UserId == notification.SenderId && x.Channel == notification.Channel);

        if (sender == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        var receiver = await _context.Users.FirstOrDefaultAsync(x => x.Id == notification.ReceiverId);

        if (receiver == null)
        {
            throw new NotFoundException(UserCouldNotBeFoundError);
        }

        var status = new NotificationStatusModel
        {
            Id = Guid.NewGuid().ToString(),
            NotificationId = notification.Id,
            Status = StatusType.Updated,
            TimeStamp = DateTime.Now,
            Message = UpdateNotificatonMessage
        };

        _context.Notifications.Update(_mapper.Map<NotificationModel, Notification>(notification));
        _context.NotificationStatuses.Add(_mapper.Map<NotificationStatusModel, NotificationStatus>(status));

        await _context.SaveChangesAsync();
    }

    public async Task CancelNotification(CancelNotificationRequestModel model)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == model.NotificatioId);
        if (notification == null)
        {
            throw new NotFoundException(NotificationCouldNotBeFoundError);
        }

        var status = await _context.NotificationStatuses.FirstOrDefaultAsync(x =>
            x.NotificationId == model.NotificatioId && x.Status == StatusType.Cancelled);
        if (status != null)
        {
            throw new NotificationException(NotificationIsCancelled);
        }

        var notificationStatus = new NotificationStatusModel
        {
            Id = Guid.NewGuid().ToString(),
            NotificationId = model.NotificatioId,
            Status = StatusType.Cancelled,
            TimeStamp = DateTime.Now,
            Message = model.Message
        };

        _context.NotificationStatuses.Add(_mapper.Map<NotificationStatusModel, NotificationStatus>(notificationStatus));
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNotification(string id)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
        if (notification == null)
        {
            throw new NotFoundException(NotificationCouldNotBeFoundError);
        }

        var statuses = await _context.NotificationStatuses.Where(x => x.NotificationId == id).ToListAsync();
        _context.Notifications.Remove(notification);
        _context.NotificationStatuses.RemoveRange(statuses);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserConfig(UserConfigModel userConfig)
    {
        userConfig.Token = EncryptionService.Encrypt(userConfig.Token);
        _context.UserConfigs.Update(_mapper.Map<UserConfigModel, UserConfig>(userConfig));
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserConfig(string id)
    {
        var userConfig = await _context.UserConfigs.FirstOrDefaultAsync(x => x.Id == id);
        if (userConfig == null)
        {
            throw new NotFoundException(UserConfigurationCouldNotBeFoundError);
        }

        _context.UserConfigs.Remove(userConfig);
        await _context.SaveChangesAsync();
    }

    public async Task SetStatus(string id, StatusType statusType, string message)
    {
        var status = new NotificationStatus
        {
            Id = Guid.NewGuid().ToString(),
            NotificationId = id,
            Status = statusType,
            TimeStamp = DateTime.Now,
            Message = message
        };

        _context.NotificationStatuses.Add(status);
        await _context.SaveChangesAsync();
    }
}