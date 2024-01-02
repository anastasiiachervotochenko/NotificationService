using NotificationService.Data.Enums;

namespace NotificationService.Domain.Contracts.Models.DomainModels;

public class NotificationStatusModel
{
    public string Id { get; set; }
    public string NotificationId { get; set; }
    public StatusType Status { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Message { get; set; }
}