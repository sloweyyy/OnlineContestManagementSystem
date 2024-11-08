using MongoDB.Driver;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Repositories
{
    public class ContestRepository : IContestRepository
    {
        private readonly IMongoCollection<Contest> _contests;

        public ContestRepository(IMongoDatabase database)
        {
            _contests = database.GetCollection<Contest>("Contests");
        }

        public async Task CreateContestAsync(Contest contest)
        {
            await _contests.InsertOneAsync(contest);
        }

        public async Task<Contest> GetContestByIdAsync(string id)
        {
            return await _contests.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Contest>> GetAllContestsAsync()
        {
            return await _contests.Find(_ => true).ToListAsync();
        }

        public async Task UpdateContestAsync(Contest contest)
        {
            await _contests.ReplaceOneAsync(c => c.Id == contest.Id, contest);
        }

        public async Task DeleteContestAsync(string id)
        {
            await _contests.DeleteOneAsync(c => c.Id == id);
        }
    }
}
