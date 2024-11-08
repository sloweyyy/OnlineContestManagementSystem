using OnlineContestManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Repositories
{
    public interface IContestRepository
    {
        Task CreateContestAsync(Contest contest);
        Task<Contest> GetContestByIdAsync(string id);
        Task UpdateContestAsync(Contest contest);
        Task DeleteContestAsync(string id);
        Task<List<Contest>> GetAllContestsAsync();
        Task<List<Contest>> SearchContestsAsync(string keyword, int? minParticipants, int? maxParticipants, List<string> skills);
    }
}
