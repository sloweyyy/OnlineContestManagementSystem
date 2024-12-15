using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using OnlineContestManagement.Models;

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

        public async Task SendRegistrationConfirmation(string to, string contestName)
        {
            if (string.IsNullOrWhiteSpace(_smtpSettings.Username))
            {
                throw new InvalidOperationException("SMTP Username is not configured");
            }


            var htmlTemplate = File.ReadAllText("Templates/RegistrationConfirmationTemplate.html");
            var emailBody = htmlTemplate
                .Replace("{{recipientName}}", "Contestant")
                .Replace("{{contestName}}", contestName);

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

        public async Task SendWithdrawalConfirmation(string to, string contestName)
        {
            var htmlTemplate = File.ReadAllText("Templates/WithdrawalConfirmationTemplate.html");
            var emailBody = htmlTemplate
            .Replace("{{contestName}}", contestName)
            .Replace("{{recipientName}}", "Contestant");
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

        public async Task SendContestUpdateNotification(string to, string orgName, string contestName, string updateType)
        {
            var htmlTemplate = File.ReadAllText("Templates/ContestUpdateNotificationTemplate.html");
            var emailBody = htmlTemplate
            .Replace("{{recipientName}}", orgName)
            .Replace("{{contestName}}", contestName)
            .Replace("{{updateType}}", updateType);
            Console.WriteLine("Sending contest update notification email to " + to);
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.FromName),
                Subject = "Contest Update Notification",
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

        public async Task SendResetPasswordEmail(string to, string resetToken)
        {
            var htmlTemplate = File.ReadAllText("Templates/ResetPasswordTemplate.html");
            // client url from env
            var resetPasswordLink = $"{Environment.GetEnvironmentVariable("CLIENT_URL")}/reset-password?resetPassword={resetToken}";

            var emailBody = htmlTemplate
            .Replace("{{resetPasswordLink}}", resetPasswordLink);
            Console.WriteLine("Sending reset password email to " + to);
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.FromName),
                Subject = "Reset Password",
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
        public async Task SendContactFormEmail(ContactFormModel model)
        {
            var adminTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ContactFormTemplate.html");
            var adminEmailBody = await File.ReadAllTextAsync(adminTemplatePath);

            adminEmailBody = adminEmailBody.Replace("{{FirstName}}", model.FirstName)
                                           .Replace("{{LastName}}", model.LastName)
                                           .Replace("{{Email}}", model.Email)
                                           .Replace("{{Subject}}", model.Subject)
                                           .Replace("{{Message}}", model.Message);

            var adminMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.FromName),
                Subject = "Contact Form Submission",
                Body = adminEmailBody,
                IsBodyHtml = true
            };
            adminMessage.To.Add("sloweycontact@gmail.com");

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.EnableSsl = _smtpSettings.UseSSL;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                await smtpClient.SendMailAsync(adminMessage);
            }

            var confirmTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ContactFormConfirmation.html");
            var confirmEmailBody = await File.ReadAllTextAsync(confirmTemplatePath);

            confirmEmailBody = confirmEmailBody.Replace("{{FirstName}}", model.FirstName)
                                               .Replace("{{LastName}}", model.LastName)
                                               .Replace("{{Subject}}", model.Subject)
                                               .Replace("{{Message}}", model.Message);

            var confirmMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, _smtpSettings.FromName),
                Subject = "Thank You for Contacting Us!",
                Body = confirmEmailBody,
                IsBodyHtml = true
            };
            confirmMessage.To.Add(model.Email);

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.EnableSsl = _smtpSettings.UseSSL;
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                await smtpClient.SendMailAsync(confirmMessage);
            }
        }



    }
}