using NotificationService.Data.Enums;

namespace NotificationService.Domain.Contracts.Models.RequestModel;

public class AddUserConfigRequestModel
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public ChannelType Channel { get; set; }
}