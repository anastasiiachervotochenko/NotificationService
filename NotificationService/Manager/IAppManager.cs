using NotificationService.Domain.Contracts.Models.DomainModels;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Contracts.Models.Result;

namespace NotificationService.Manager;

public interface IAppManager
{
    public Task<List<NotificationResponseModel>> GetNotifications();
    public Task<NotificationResponseModel> GetNotificationById(string id);
    public Task<List<UserModel>> GetAllUsers();
    public Task<UserModel> GetUserById(string id);
    public Task CreateNotification(CreateNotificationRequestModel notification);
    public Task CreateUser(CreateUserRequestModel userModel);
    public Task AddUserConfig(AddUserConfigRequestModel userConfigRequestModel);
    public Task SendNotification(string id);
    public Task UpdateNotification(NotificationModel notification);
    public Task CancelNotification(CancelNotificationRequestModel model);
    public Task DeleteNotification(string id);
    public Task UpdateUserConfig(UserConfigModel userConfig);
    public Task DeleteUserConfig(string id);
}