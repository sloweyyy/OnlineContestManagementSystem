using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Data.Repositories
{
  public interface IContestRepository
  {
    Task CreateContestAsync(Contest contest);
    Task<Contest> GetContestByIdAsync(string id);
    Task<Contest> UpdateContestAsync(string id, Contest contest);
    Task DeleteContestAsync(string id);
    Task<List<Contest>> SearchContestsAsync(ContestSearchFilter filter);
  }
}