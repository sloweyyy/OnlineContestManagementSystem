using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IContestRegistrationService
    {
        Task<RegistrationResult> RegisterUserForContestAsync(string contestId, RegisterForContestModel registrationModel);
        Task<bool> WithdrawUserFromContestAsync(WithdrawFromContestModel withdrawModel);
        Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter);
        Task<List<ContestRegistration>> GetContestsByUserIdAsync(string userId);
    }

}
