using NotificationService.Domain.Contracts.Models.DomainModels;

namespace NotificationService.Domain.Contracts.Models.Result;

public class SendNotificationModel
{
    public NotificationModel Notification { get; set; }
    public SenderResponseModel Sender { get; set; }
    public UserModel Receiver { get; set; }
}