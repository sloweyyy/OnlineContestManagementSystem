using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Data.Repositories
{
  public interface IContestRepository
  {
    Task CreateContestAsync(Contest contest);
    Task<List<Contest>> GetAllContestsAsync();
    Task<Contest> GetContestByIdAsync(string id);
    Task<Contest> UpdateContestAsync(string id, Contest contest);
    Task DeleteContestAsync(string id);
    Task<List<Contest>> SearchContestsAsync(ContestSearchFilter filter);
    Task<int> CountContestsByDateAsync(DateTime date);
    }
}