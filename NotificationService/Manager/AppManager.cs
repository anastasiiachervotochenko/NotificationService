using FluentValidation;
using NotificationService.Data.Enums;
using NotificationService.Domain.Contracts.Interfaces;
using NotificationService.Domain.Contracts.Models.DomainModels;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Contracts.Models.Result;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Manager;

public class AppManager : IAppManager
{
    private readonly IMailService _mailService;
    private readonly IAppService _appService;

    private readonly IValidator<CreateUserRequestModel> _createUserValidator;
    private readonly IValidator<CreateNotificationRequestModel> _createNotificationValidator;

    private const string SendEmailError = "Failed to send notification via email.";
    private const string NotificationSuccess = "Notification was successfully sent.";
    private const string NotificationError = "Notification wasn't sent. With Error: ";


    public AppManager(IMailService mailService, IAppService appService,
        IValidator<CreateUserRequestModel> createUserValidator,
        IValidator<CreateNotificationRequestModel> createNotificationValidator)
    {
        _mailService = mailService;
        _appService = appService;
        _createUserValidator = createUserValidator;
        _createNotificationValidator = createNotificationValidator;
    }

    public async Task<List<NotificationResponseModel>> GetNotifications()
    {
        var result = await _appService.GetNotifications();

        return result;
    }

    public async Task<NotificationResponseModel> GetNotificationById(string id)
    {
        var result = await _appService.GetNotificationById(id);

        return result;
    }

    public async Task<List<UserModel>> GetAllUsers()
    {
        var result = await _appService.GetAllUsers();

        return result;
    }

    public async Task<UserModel> GetUserById(string id)
    {
        var result = await _appService.GetUserById(id);

        return result;
    }


    public async Task CreateNotification(CreateNotificationRequestModel notification)
    {
        var validationResult = await _createNotificationValidator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _appService.CreateNotification(notification);
    }

    public async Task CreateUser(CreateUserRequestModel userModel)
    {
        var validationResult = await _createUserValidator.ValidateAsync(userModel);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _appService.CreateUser(userModel);
    }

    public async Task AddUserConfig(AddUserConfigRequestModel userConfigRequestModel)
    {
        var userConfigModel = new UserConfigModel
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userConfigRequestModel.UserId,
            Token = userConfigRequestModel.Token,
            Channel = userConfigRequestModel.Channel
        };

        await _appService.AddUserConfig(userConfigModel);
    }

    public async Task SendNotification(string id)
    {
        try
        {
            var notificationDataForSend = await _appService.GetNotificationDataForSend(id);

            if (notificationDataForSend.Notification.Channel == ChannelType.Email)
            {
                await _mailService.SendEmailAsync(notificationDataForSend);
            }
            else
            {
                throw new SendNotificationException("Fail");
            }

            await _appService.SetStatus(id, StatusType.Success, NotificationSuccess);
        }
        catch (SendEmailException ex)
        {
            await _appService.SetStatus(id, StatusType.Error, NotificationError + SendEmailError);
            throw new SendEmailException(ex.Message);
        }
    }

    public async Task UpdateNotification(NotificationModel notification)
    {
        await _appService.UpdateNotification(notification);
    }

    public async Task CancelNotification(CancelNotificationRequestModel model)
    {
        await _appService.CancelNotification(model);
    }

    public async Task DeleteNotification(string id)
    {
        await _appService.DeleteNotification(id);
    }

    public async Task UpdateUserConfig(UserConfigModel userConfig)
    {
        await _appService.UpdateUserConfig(userConfig);
    }

    public async Task DeleteUserConfig(string id)
    {
        await _appService.DeleteUserConfig(id);
    }
}