using System.Threading.Tasks;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IDashboardService
    {
        Task<ContestStatisticsModel> GetContestStatisticsAsync();
        Task<RegistrationStatisticsModel> GetRegistrationStatisticsAsync();
        Task<int> GetTotalContestsAsync();
        Task<Dictionary<string, List<ContestRegistration>>> GetContestParticipantsAsync();
        Task<decimal> GetContestRevenueAsync();
        Task<decimal> GetWebsiteRevenueAsync();
        Task<int> GetTotalParticipantsAsync();
        Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync();
        Task<List<FeaturedContest>> GetFeaturedContestsAsync(int topN = 5);

    }
}
