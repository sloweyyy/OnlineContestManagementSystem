using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IContestService
    {
        Task CreateContestAsync(Contest contest);
        Task<Contest> GetContestByIdAsync(string id);
        Task UpdateContestAsync(Contest contest);
        Task DeleteContestAsync(string id);
        Task<List<Contest>> SearchContestsAsync(string keyword, int? minParticipants, int? maxParticipants, List<string> skills);
    }
}
