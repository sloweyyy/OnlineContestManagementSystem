using System.Net.Mail;
using System.Net;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendRegistrationConfirmation(string email, string contestId)
        {
            var subject = "Registration Successful";
            var body = $"You have successfully registered for contest {contestId}. Thank you for participating!";

            using (var smtpClient = new SmtpClient("smtp.your-email-provider.com"))
            {
                smtpClient.Port = 587; // Hoặc cổng phù hợp
                smtpClient.Credentials = new NetworkCredential("your-email@example.com", "your-email-password");
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@example.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
