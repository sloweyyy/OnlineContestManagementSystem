using MongoDB.Bson;
using MongoDB.Driver;
using OnlineContestManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineContestManagement.Repositories
{
    public class ContestRepository : IContestRepository
    {
        private readonly IMongoCollection<Contest> _contests;

        public ContestRepository(IMongoDatabase database)
        {
            _contests = database.GetCollection<Contest>("Contests");
        }

        public async Task CreateContestAsync(Contest contest) => await _contests.InsertOneAsync(contest);

        public async Task<Contest> GetContestByIdAsync(string id) =>
            await _contests.Find(contest => contest.Id == id).FirstOrDefaultAsync();

        public async Task UpdateContestAsync(Contest contest) =>
            await _contests.ReplaceOneAsync(c => c.Id == contest.Id, contest);

        public async Task DeleteContestAsync(string id) =>
            await _contests.DeleteOneAsync(contest => contest.Id == id);

        public async Task<List<Contest>> GetAllContestsAsync() =>
            await _contests.Find(_ => true).ToListAsync();

        public async Task<List<Contest>> SearchContestsAsync(string keyword, int? minParticipants, int? maxParticipants, List<string> skills)
        {
            var filter = Builders<Contest>.Filter.Empty;

            if (!string.IsNullOrEmpty(keyword))
            {
                var keywordFilter = Builders<Contest>.Filter.Or(
                    Builders<Contest>.Filter.Regex("Name", new BsonRegularExpression(keyword, "i")),
                    Builders<Contest>.Filter.Regex("Description", new BsonRegularExpression(keyword, "i"))
                );
                filter &= keywordFilter;
            }

            if (minParticipants.HasValue)
            {
                filter &= Builders<Contest>.Filter.Gte(c => c.MinParticipants, minParticipants.Value);
            }

            if (maxParticipants.HasValue)
            {
                filter &= Builders<Contest>.Filter.Lte(c => c.MaxParticipants, maxParticipants.Value);
            }

            if (skills != null && skills.Any())
            {
                filter &= Builders<Contest>.Filter.All(c => c.Skills, skills);
            }

            return await _contests.Find(filter).ToListAsync();
        }
    }
}
