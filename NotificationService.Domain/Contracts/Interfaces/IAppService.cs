using NotificationService.Data.Enums;
using NotificationService.Domain.Contracts.Models.DomainModels;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Contracts.Models.Result;

namespace NotificationService.Domain.Contracts.Interfaces;

public interface IAppService
{
    public Task CreateUser(CreateUserRequestModel userModel);
    public Task CreateNotification(CreateNotificationRequestModel notification);
    public Task<List<NotificationResponseModel>> GetNotifications();
    public Task<List<UserModel>> GetAllUsers();
    public Task<UserModel> GetUserById(string id);
    public Task AddUserConfig(UserConfigModel userConfigModel);
    public Task<NotificationResponseModel> GetNotificationById(string id);
    public Task<SendNotificationModel> GetNotificationDataForSend(string id);
    public Task UpdateNotification(NotificationModel notification);
    public Task CancelNotification(CancelNotificationRequestModel model);
    public Task DeleteNotification(string id);
    public Task UpdateUserConfig(UserConfigModel userConfig);
    public Task DeleteUserConfig(string id);
    public Task SetStatus(string id, StatusType statusType, string message);
}