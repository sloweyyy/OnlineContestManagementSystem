namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmation(string email, string contestId);
        Task SendWithdrawalConfirmation(string email, string contestId);
        Task SendContestUpdateNotification(string email, string orgName, string contestName, string updateType);
    }


}
