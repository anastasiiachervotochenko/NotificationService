using NotificationService.Data.Enums;

namespace NotificationService.Domain.Contracts.Models.DomainModels;

public class UserConfigModel
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public ChannelType Channel { get; set; }
}