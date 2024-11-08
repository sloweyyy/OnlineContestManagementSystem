using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IContestService
    {
        Task CreateContestAsync(Contest contest);
        Task<Contest> GetContestByIdAsync(string id);
        Task<IEnumerable<Contest>> GetAllContestsAsync();
        Task UpdateContestAsync(Contest contest);
        Task DeleteContestAsync(string id);
    }
}
