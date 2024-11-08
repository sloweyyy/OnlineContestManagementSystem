using OnlineContestManagement.Models;
using OnlineContestManagement.Repositories;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class ContestService : IContestService
    {
        private readonly IContestRepository _contestRepository;

        public ContestService(IContestRepository contestRepository)
        {
            _contestRepository = contestRepository;
        }

        public async Task CreateContestAsync(Contest contest)
        {
            await _contestRepository.CreateContestAsync(contest);
        }

        public async Task<Contest> GetContestByIdAsync(string id)
        {
            return await _contestRepository.GetContestByIdAsync(id);
        }

        public async Task<IEnumerable<Contest>> GetAllContestsAsync()
        {
            return await _contestRepository.GetAllContestsAsync();
        }

        public async Task UpdateContestAsync(Contest contest)
        {
            await _contestRepository.UpdateContestAsync(contest);
        }

        public async Task DeleteContestAsync(string id)
        {
            await _contestRepository.DeleteContestAsync(id);
        }
    }
}
