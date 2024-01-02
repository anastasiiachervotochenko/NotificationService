namespace NotificationService.Domain.Contracts.Models.Result;

public class SenderResponseModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}