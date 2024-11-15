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

    public async Task<bool> WithdrawUserAsync(string contestId, string userId)
    {
        var result = await _collection.DeleteOneAsync(r => r.ContestId == contestId && ObjectId.Parse(r.UserId) == ObjectId.Parse(userId));
        return result.DeletedCount > 0;
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
        return await _collection.Find(filter).ToListAsync(); // Sử dụng _collection thay vì _registrations
    }

    public async Task<List<Contest>> GetContestsByUserIdAsync(string userId)
    {
        var filter = Builders<ContestRegistration>.Filter.Eq(r => r.UserId, userId);
        var registrations = await _collection.Find(filter).ToListAsync();

        var contestIds = registrations.ConvertAll(r => r.ContestId);
        var contestFilter = Builders<Contest>.Filter.In(c => c.Id, contestIds);
        return await _contestCollection.Find(contestFilter).ToListAsync();
    }
}
