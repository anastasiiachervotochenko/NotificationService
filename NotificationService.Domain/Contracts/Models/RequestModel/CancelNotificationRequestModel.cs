namespace NotificationService.Domain.Contracts.Models.RequestModel;

public class CancelNotificationRequestModel
{
    public string NotificatioId { get; set; }
    public string Message { get; set; }
}