using System.Net;
using System.Net.Mail;
using NotificationService.Domain.Contracts.Interfaces;
using NotificationService.Domain.Contracts.Models.Result;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Domain.Services.Email;

public class MailService : IMailService
{
    public async Task SendEmailAsync(SendNotificationModel model)
    {
        try
        {
            var token = EncryptionService.Decrypt(model.Sender.Token);
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(model.Sender.Email, token)
            };

            await client.SendMailAsync(
                new MailMessage(from: model.Sender.Email,
                    to: model.Receiver.Email,
                    model.Notification.Title,
                    model.Notification.Body
                ));
        }
        catch (Exception ex)
        {
            throw new SendEmailException(ex.Message);
        }
    }
}