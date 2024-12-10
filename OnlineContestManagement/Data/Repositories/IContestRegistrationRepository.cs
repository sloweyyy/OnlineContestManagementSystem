using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public interface IContestRegistrationRepository
    {
        Task<bool> RegisterUserAsync(ContestRegistration registration);
        Task WithdrawUserAsync(string contestId, string userId);
        Task<List<ContestRegistration>> GetRegistrationsByContestIdAsync(string contestId);
        Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter);
        Task<List<ContestRegistration>> GetRegistrationsByUserIdAsync(string userId);
        Task<List<Contest>> GetContestsByUserIdAsync(string userId);
        Task<ContestRegistration> GetRegistrationByUserIdAndContestIdAsync(string contestId, string userId);
        Task<int> CountRegistrationsByDateAsync(DateTime date);
        Task<Dictionary<string, List<ContestRegistration>>> GetContestParticipantsAsync();
        Task<int> GetTotalParticipantsAsync();
        Task<List<FeaturedContest>> GetFeaturedContestsAsync(int topN = 5);
        Task UpdateRegistrationAsync(ContestRegistration registration);




    }

}
