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
    private readonly ILogger<ContestRepository> _logger;


    public ContestRepository(IMongoDatabase database, ILogger<ContestRepository> logger)
    {
      _contests = database.GetCollection<Contest>("Contests");
      _logger = logger;

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
          .Set(c => c.ImageUrl, contest.ImageUrl)
          .Set(c => c.EntryFee, contest.EntryFee)
          .Set(c => c.Status, contest.Status);

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

    public async Task<int> GetTotalContestsAsync()
    {
      return (int)await _contests.CountDocumentsAsync(Builders<Contest>.Filter.Empty);
    }
    public async Task<List<QuarterlyContestStatusCountResponse>> GetQuarterlyContestCountsAsync()
    {
      var filter = Builders<Contest>.Filter.Eq(c => c.Status, "approved");
      var contests = await _contests.Find(filter).ToListAsync();

      _logger.LogInformation($"Total approved contests fetched: {contests.Count}");

      if (contests == null || !contests.Any())
      {
        _logger.LogWarning("No approved contests found.");
        return GenerateEmptyQuarterCounts();
      }

      var minYear = contests.Min(c => c.StartDate.Year);
      var maxYear = contests.Max(c => c.StartDate.Year);

      var contestsWithStatus = contests.Select(c => new
      {
        Year = c.StartDate.Year,
        Quarter = GetQuarter(c.StartDate),
        Status = DetermineContestStatus(c.StartDate, c.EndDate)
      }).ToList();

      _logger.LogInformation("Contest statuses determined based on dates.");

      var allQuarterCombinations = GenerateAllQuarterCombinations(minYear, maxYear);

      var quarterCounts = allQuarterCombinations.GroupJoin(
          contestsWithStatus,
          combo => new { combo.Year, combo.Quarter },
          contest => new { contest.Year, contest.Quarter },
          (combo, contestGroup) => new QuarterlyContestStatusCountResponse
          {
            Year = combo.Year,
            Quarter = combo.Quarter,
            Status = contestGroup.Any() ? contestGroup.First().Status : "Không có cuộc thi",
            ContestCount = contestGroup.Count()
          })
          .ToList();

      _logger.LogInformation($"Quarter counts calculated: {quarterCounts.Count}");

      return quarterCounts;
    }
    private List<(int Year, string Quarter)> GenerateAllQuarterCombinations(int minYear, int maxYear)
    {
      var combinations = new List<(int Year, string Quarter)>();

      for (int year = minYear; year <= maxYear; year++)
      {
        combinations.Add((year, "Q1"));
        combinations.Add((year, "Q2"));
        combinations.Add((year, "Q3"));
        combinations.Add((year, "Q4"));
      }

      return combinations;
    }

    private string GetQuarter(DateTime date)
    {
      int month = date.Month;
      if (month <= 3) return "Q1";
      if (month <= 6) return "Q2";
      if (month <= 9) return "Q3";
      return "Q4";
    }
    private string DetermineContestStatus(DateTime startDate, DateTime endDate)
    {
      var now = DateTime.UtcNow;

      if (now < startDate)
        return "Sắp diễn ra";
      else if (now >= startDate && now <= endDate)
        return "Đang diễn ra";
      else
        return "Đã kết thúc";
    }

    private List<QuarterlyContestStatusCountResponse> GenerateEmptyQuarterCounts()
    {
      var currentYear = DateTime.UtcNow.Year;

      return new List<QuarterlyContestStatusCountResponse>
    {
        new QuarterlyContestStatusCountResponse { Year = currentYear, Quarter = "Q1", Status = "Không có cuộc thi", ContestCount = 0 },
        new QuarterlyContestStatusCountResponse { Year = currentYear, Quarter = "Q2", Status = "Không có cuộc thi", ContestCount = 0 },
        new QuarterlyContestStatusCountResponse { Year = currentYear, Quarter = "Q3", Status = "Không có cuộc thi", ContestCount = 0 },
        new QuarterlyContestStatusCountResponse { Year = currentYear, Quarter = "Q4", Status = "Không có cuộc thi", ContestCount = 0 }
    };
    }
  }
}