using OnlineContestManagement.Data.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using OnlineContestManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineContestManagement.Data.Repositories
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

    public async Task<List<Contest>> GetAllContestsAsync()
    {
      return await _contests.Find(Builders<Contest>.Filter.Empty).ToListAsync();
    }


    public async Task<Contest> UpdateContestAsync(string id, Contest contest)
    {
      var filter = Builders<Contest>.Filter.Eq(c => c.Id, id);
      var update = Builders<Contest>.Update
          .Set(c => c.Name, contest.Name)
          .Set(c => c.RuleDescription, contest.RuleDescription)
          .Set(c => c.StartDate, contest.StartDate)
          .Set(c => c.EndDate, contest.EndDate)
          .Set(c => c.MinimumParticipant, contest.MinimumParticipant)
          .Set(c => c.MaximumParticipant, contest.MaximumParticipant)
          .Set(c => c.Prizes, contest.Prizes)
          .Set(c => c.ParticipantInformationRequirements, contest.ParticipantInformationRequirements)
          .Set(c => c.OrganizationInformation, contest.OrganizationInformation)
          .Set(c => c.ImageUrl, contest.ImageUrl);

      var result = await _contests.UpdateOneAsync(filter, update);
      if (result.ModifiedCount == 1)
      {
        return await GetContestByIdAsync(id);
      }
      return null;
    }

    public async Task DeleteContestAsync(string id)
    {
      var filter = Builders<Contest>.Filter.Eq(c => c.Id, id);
      await _contests.DeleteOneAsync(filter);
    }

    public async Task<List<Contest>> SearchContestsAsync(ContestSearchFilter filter)
    {
      var builder = Builders<Contest>.Filter.Empty;

      if (!string.IsNullOrEmpty(filter.Name))
      {
        builder &= Builders<Contest>.Filter.Regex(c => c.Name, new BsonRegularExpression(filter.Name, "i"));
      }

      if (filter.StartDate.HasValue)
      {
        builder &= Builders<Contest>.Filter.Gte(c => c.StartDate, filter.StartDate.Value);
      }

      if (filter.EndDate.HasValue)
      {
        builder &= Builders<Contest>.Filter.Lte(c => c.EndDate, filter.EndDate.Value);
      }

      if (!string.IsNullOrEmpty(filter.Status))
      {
        switch (filter.Status.ToLower())
        {
          case "open":
            builder &= Builders<Contest>.Filter.Lt(c => c.StartDate, DateTime.Now);
            break;
          case "closed":
            builder &= Builders<Contest>.Filter.Gt(c => c.EndDate, DateTime.Now);
            break;
          case "ongoing":
            builder &= Builders<Contest>.Filter.Gte(c => c.StartDate, DateTime.Now) & Builders<Contest>.Filter.Lte(c => c.EndDate, DateTime.Now);
            break;
        }
      }

      return await _contests.Find(builder).ToListAsync();
    }
    public async Task<int> CountContestsByDateAsync(DateTime date)
    {
      var filter = Builders<Contest>.Filter.And(
          Builders<Contest>.Filter.Gte(c => c.StartDate, date.Date),
          Builders<Contest>.Filter.Lt(c => c.StartDate, date.Date.AddDays(1))
      );

      return (int)await _contests.CountDocumentsAsync(filter);
    }



    public async Task<List<Contest>> GetContestsByCreatorIdAsync(string creatorId)
    {
      return await _contests.Find(c => c.CreatorUserId == creatorId).ToListAsync();
    }
  }
}