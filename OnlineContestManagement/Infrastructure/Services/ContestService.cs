using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System.Security.Claims;


namespace OnlineContestManagement.Infrastructure.Services
{
  public class ContestService : IContestService
  {
    private readonly IContestRepository _contestRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public ContestService(IContestRepository contestRepository, IHttpContextAccessor httpContextAccessor)
    {
      _contestRepository = contestRepository ?? throw new ArgumentNullException(nameof(contestRepository));
      _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    }

    public async Task<Contest> CreateContestAsync(CreateContestModel model)
    {
      var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

      var contest = new Contest
      {
        Name = model.Name,
        Description = model.Description,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        MinimumParticipant = model.MinimumParticipant,
        MaximumParticipant = model.MaximumParticipant,
        Prizes = model.Prizes,
        ParticipantInformationRequirements = model.ParticipantInformationRequirements,
        CreatorUserId = userId
      };

      await _contestRepository.CreateContestAsync(contest);
      return contest;
    }

    public async Task<Contest> GetContestDetailsAsync(string id)
    {
      return await _contestRepository.GetContestByIdAsync(id);
    }

    public async Task<Contest> UpdateContestAsync(string id, UpdateContestModel model)
    {
      var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
      var contest = new Contest
      {
        Id = id,
        Name = model.Name,
        Description = model.Description,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        MinimumParticipant = model.MinimumParticipant,
        MaximumParticipant = model.MaximumParticipant,
        Prizes = model.Prizes,
        ParticipantInformationRequirements = model.ParticipantInformationRequirements,
        CreatorUserId = userId
      };

      return await _contestRepository.UpdateContestAsync(id, contest);
    }

    public async Task DeleteContestAsync(string id)
    {
      await _contestRepository.DeleteContestAsync(id);
    }

    public async Task<List<Contest>> SearchContestsAsync(ContestSearchFilter filter)
    {
      return await _contestRepository.SearchContestsAsync(filter);
    }
  }
}