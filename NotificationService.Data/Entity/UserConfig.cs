using NotificationService.Data.Enums;

namespace NotificationService.Data.Entity;

public class UserConfig
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public ChannelType Channel { get; set; }
}