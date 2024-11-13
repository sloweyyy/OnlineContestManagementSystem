using OnlineContestManagement.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using OnlineContestManagement.Data.Models;
using MongoDB.Driver.Linq;

namespace OnlineContestManagement.Data.Repositories
{
    public class ContestRegistrationRepository : IContestRegistrationRepository
    {
        private readonly IMongoCollection<ContestRegistration> _registrations;

        public ContestRegistrationRepository(IMongoDatabase database)
        {
            _registrations = database.GetCollection<ContestRegistration>("ContestRegistrations");
        }

        public async Task<bool> RegisterUserAsync(ContestRegistration registration)
        {
            await _registrations.InsertOneAsync(registration);
            return true;
        }

        public async Task<bool> WithdrawUserAsync(string contestId, string userId)
        {
            var result = await _registrations.DeleteOneAsync(r => r.ContestId == contestId && r.UserId == userId);
            return result.DeletedCount > 0;
        }

        public async Task<List<ContestRegistration>> GetRegistrationsByContestIdAsync(string contestId)
        {
            return await _registrations.Find(r => r.ContestId == contestId).ToListAsync();
        }

        public async Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter)
        {
            var query = _registrations.AsQueryable();

            if (!string.IsNullOrEmpty(filter.ContestId))
            {
                query = query.Where(r => r.ContestId == filter.ContestId);
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                query = query.Where(r => r.UserId == filter.UserId);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(r => r.Status == filter.Status);
            }

            return await query.ToListAsync();
        }
    }
}
