namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmation(string email, string contestId);
        Task SendWithdrawalConfirmation(string email, string contestId);
    }


}
