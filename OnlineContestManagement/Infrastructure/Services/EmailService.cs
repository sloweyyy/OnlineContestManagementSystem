using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            Console.WriteLine($"SMTP Username: {_smtpSettings.Username}");
            Console.WriteLine($"SMTP FromName: {_smtpSettings.FromName}");
        }

        public async Task SendRegistrationConfirmation(string to, string contestId)
        {
            if (string.IsNullOrWhiteSpace(_smtpSettings.Username))
            {
                throw new InvalidOperationException("SMTP Username is not configured");
            }


            var htmlTemplate = File.ReadAllText("Templates/RegistrationConfirmationTemplate.html");
            var emailBody = htmlTemplate
                .Replace("{{recipientName}}", "Contestant")
                .Replace("{{contestId}}", contestId);

            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.FromName ?? _smtpSettings.Username),
                Subject = "Registration Confirmation",
                Body = emailBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                smtpClient.EnableSsl = _smtpSettings.UseSSL;
                await smtpClient.SendMailAsync(message);
            }
        }

        public async Task SendWithdrawalConfirmation(string to, string contestId)
        {
            var htmlTemplate = File.ReadAllText("Templates/WithdrawalConfirmationTemplate.html");
            var emailBody = htmlTemplate.Replace("{{contestId}}", contestId);
            Console.WriteLine("Sending withdrawal confirmation email to " + to);
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.FromName),
                Subject = "Withdrawal Confirmation",
                Body = emailBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.EnableSsl = _smtpSettings.UseSSL;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                await smtpClient.SendMailAsync(message);
            }
        }

    }
}