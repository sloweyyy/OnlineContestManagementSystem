using System.Threading.Tasks;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IDashboardService
    {
        Task<ContestStatisticsModel> GetContestStatisticsAsync();
        Task<RegistrationStatisticsModel> GetRegistrationStatisticsAsync();
    }
}
