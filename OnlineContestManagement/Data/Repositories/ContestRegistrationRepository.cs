using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;

public class ContestRegistrationRepository : IContestRegistrationRepository
{
    private readonly IMongoCollection<ContestRegistration> _collection;
    private readonly IMongoCollection<Contest> _contestCollection;

    public ContestRegistrationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ContestRegistration>("contestRegistrations");
        _contestCollection = database.GetCollection<Contest>("contests");
    }

    public async Task<bool> RegisterUserAsync(ContestRegistration registration)
    {
        await _collection.InsertOneAsync(registration);
        return true;
    }

    public async Task WithdrawUserAsync(string contestId, string userId)
    {
        var filter = Builders<ContestRegistration>.Filter.And(
            Builders<ContestRegistration>.Filter.Eq(cr => cr.ContestId, contestId),
            Builders<ContestRegistration>.Filter.Eq(cr => cr.UserId, userId)
        );

        var update = Builders<ContestRegistration>.Update
            .Set(cr => cr.Status, "Withdrawn")
            .Set(cr => cr.WithdrawalDate, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task<List<ContestRegistration>> GetRegistrationsByContestIdAsync(string contestId)
    {
        return await _collection.Find(r => r.ContestId == contestId).ToListAsync();
    }

    public async Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter)
    {
        var query = _collection.AsQueryable();

        if (!string.IsNullOrEmpty(filter.ContestId))
        {
            query = query.Where(r => r.ContestId == filter.ContestId);
        }

        if (!string.IsNullOrEmpty(filter.UserId))
        {
            query = query.Where(r => ObjectId.Parse(r.UserId) == ObjectId.Parse(filter.UserId));
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            query = query.Where(r => r.Status == filter.Status);
        }

        return await query.ToListAsync();
    }

    public async Task<List<ContestRegistration>> GetRegistrationsByUserIdAsync(string userId)
    {
        var filter = Builders<ContestRegistration>.Filter.Eq(r => r.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<Contest>> GetContestsByUserIdAsync(string userId)
    {
        var filter = Builders<ContestRegistration>.Filter.Eq(r => r.UserId, userId);
        var registrations = await _collection.Find(filter).ToListAsync();

        var contestIds = registrations.ConvertAll(r => r.ContestId);
        var contestFilter = Builders<Contest>.Filter.In(c => c.Id, contestIds);
        return await _contestCollection.Find(contestFilter).ToListAsync();
    }

    public async Task<ContestRegistration> GetRegistrationByUserIdAndContestIdAsync(string contestId, string userId)
    {
        var filter = Builders<ContestRegistration>.Filter.Eq(r => r.ContestId, contestId) & Builders<ContestRegistration>.Filter.Eq(r => r.UserId, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<int> CountRegistrationsByDateAsync(DateTime date)
    {
        var filter = Builders<ContestRegistration>.Filter.And(
            Builders<ContestRegistration>.Filter.Gte(r => r.RegistrationDate, date.Date),
            Builders<ContestRegistration>.Filter.Lt(r => r.RegistrationDate, date.Date.AddDays(1))
        );

        return (int)await _collection.CountDocumentsAsync(filter);
    }

}
