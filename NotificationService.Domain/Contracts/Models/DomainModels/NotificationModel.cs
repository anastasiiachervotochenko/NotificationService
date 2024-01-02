using NotificationService.Data.Enums;

namespace NotificationService.Domain.Contracts.Models.DomainModels;

public class NotificationModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public ChannelType Channel { get; set; }
}