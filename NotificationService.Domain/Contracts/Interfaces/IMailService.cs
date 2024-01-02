using NotificationService.Domain.Contracts.Models.Result;

namespace NotificationService.Domain.Contracts.Interfaces;

public interface IMailService
{
    public Task SendEmailAsync(SendNotificationModel model);
}