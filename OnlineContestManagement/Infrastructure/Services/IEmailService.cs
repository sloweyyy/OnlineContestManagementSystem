namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmation(string email, string contestName);
        Task SendWithdrawalConfirmation(string email, string contestName);
        Task SendContestUpdateNotification(string email, string orgName, string contestName, string updateType);
        Task SendResetPasswordEmail(string email, string resetToken);
    }
}
