namespace NotificationService.Domain.Exceptions;

public class SendNotificationException : Exception
{
    public SendNotificationException(string message) : base(message)
    {
    }
}