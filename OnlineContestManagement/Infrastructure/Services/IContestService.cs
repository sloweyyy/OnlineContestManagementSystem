using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
  public interface IContestService
  {
    Task<Contest> CreateContestAsync(CreateContestModel model);
    Task<List<Contest>> GetAllContestsAsync();
    Task<Contest> GetContestDetailsAsync(string id);
    Task<Contest> UpdateContestAsync(string id, UpdateContestModel model);
    Task DeleteContestAsync(string id);
    Task<List<Contest>> SearchContestsAsync(ContestSearchFilter filter);
  }
}