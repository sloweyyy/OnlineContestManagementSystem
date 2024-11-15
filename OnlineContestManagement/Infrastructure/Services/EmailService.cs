using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendRegistrationConfirmation(string from, string to)
        {
            var host = _configuration["SmtpSettings:Host"];
            var port = int.Parse(_configuration["SmtpSettings:Port"]);
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var useSsl = bool.Parse(_configuration["SmtpSettings:UseSSL"]);

            var message = new MailMessage(from, to)
            {
                Subject = "Registration Confirmation",
                Body = "Thank you for registering!"
            };

            using (var smtpClient = new SmtpClient(host, port))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = useSsl;
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
