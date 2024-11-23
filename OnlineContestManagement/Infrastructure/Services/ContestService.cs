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
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;


    public ContestService(IContestRepository contestRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IEmailService emailService)
    {
      _contestRepository = contestRepository ?? throw new ArgumentNullException(nameof(contestRepository));
      _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<Contest> CreateContestAsync(CreateContestModel model)
    {
      var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

      var contest = new Contest
      {
        Name = model.Name,
        RuleDescription = model.RuleDescription,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        MinimumParticipant = model.MinimumParticipant,
        MaximumParticipant = model.MaximumParticipant,
        Prizes = model.Prizes,
        ParticipantInformationRequirements = model.ParticipantInformationRequirements,
        CreatorUserId = userId,
        OrganizationInformation = model.OrganizationInformation,
        ImageUrl = model.ImageUrl,
        EntryFee = model.EntryFee
      };

      await _contestRepository.CreateContestAsync(contest);
      return contest;
    }

    public async Task<List<Contest>> GetAllContestsAsync()
    {
      return await _contestRepository.GetAllContestsAsync();
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
        RuleDescription = model.RuleDescription,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        MinimumParticipant = model.MinimumParticipant,
        MaximumParticipant = model.MaximumParticipant,
        Prizes = model.Prizes,
        ParticipantInformationRequirements = model.ParticipantInformationRequirements,
        CreatorUserId = userId,
        OrganizationInformation = model.OrganizationInformation,
        ImageUrl = model.ImageUrl,
        EntryFee = model.EntryFee
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

    public async Task<List<Contest>> GetContestsByCreatorIdAsync(string creatorId)
    {
      return await _contestRepository.GetContestsByCreatorIdAsync(creatorId);
    }
    public async Task ApproveContestAsync(string id)
    {
      var contest = await _contestRepository.GetContestByIdAsync(id);
      if (contest == null)
      {
        throw new Exception("Contest not found.");
      }
      contest.Status = "approved";
      await _contestRepository.UpdateContestAsync(id, contest);
      await _emailService.SendContestUpdateNotification(contest.OrganizationInformation.OrgEmail, contest.OrganizationInformation.OrgName, contest.Name, "approved");
    }

    public async Task RejectContestAsync(string id)
    {
      var contest = await _contestRepository.GetContestByIdAsync(id);
      if (contest == null)
      {
        throw new Exception("Contest not found.");
      }
      contest.Status = "rejected";
      await _contestRepository.UpdateContestAsync(id, contest);
      await _emailService.SendContestUpdateNotification(contest.OrganizationInformation.OrgEmail, contest.OrganizationInformation.OrgName, contest.Name, "rejected");
    }

  }
}