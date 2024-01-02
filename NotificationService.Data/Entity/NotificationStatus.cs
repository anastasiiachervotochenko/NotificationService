using NotificationService.Data.Enums;

namespace NotificationService.Data.Entity;

public class NotificationStatus
{
    public string Id { get; set; }
    public string NotificationId { get; set; }
    public StatusType Status { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Message { get; set; }
}