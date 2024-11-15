using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public interface IContestRegistrationRepository
    {
        Task<bool> RegisterUserAsync(ContestRegistration registration);
        Task<bool> WithdrawUserAsync(string contestId, string userId);
        Task<List<ContestRegistration>> GetRegistrationsByContestIdAsync(string contestId);
        Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter);
        Task<List<ContestRegistration>> GetRegistrationsByUserIdAsync(string userId);

    }

}
