using NotificationService.Data.Enums;

namespace NotificationService.Domain.Contracts.Models.Result;

public class NotificationResponseModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public ChannelType Channel { get; set; }
    public StatusType Status { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Message { get; set; }
}