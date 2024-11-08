using OnlineContestManagement.Models;
using OnlineContestManagement.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class ContestService : IContestService
    {
        private readonly IContestRepository _contestRepository;

        public ContestService(IContestRepository contestRepository)
        {
            _contestRepository = contestRepository;
        }

        public Task CreateContestAsync(Contest contest) => _contestRepository.CreateContestAsync(contest);

        public Task<Contest> GetContestByIdAsync(string id) => _contestRepository.GetContestByIdAsync(id);

        public Task UpdateContestAsync(Contest contest) => _contestRepository.UpdateContestAsync(contest);

        public Task DeleteContestAsync(string id) => _contestRepository.DeleteContestAsync(id);

        public Task<List<Contest>> SearchContestsAsync(string keyword, int? minParticipants, int? maxParticipants, List<string> skills)
        {
            return _contestRepository.SearchContestsAsync(keyword, minParticipants, maxParticipants, skills);
        }

        public Task<IEnumerable<Contest>> GetAllContestsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
