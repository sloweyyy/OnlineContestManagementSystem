using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System.Security.Claims;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace OnlineContestManagement.Infrastructure.Services
{
  public class ContestService : IContestService
  {
    private readonly IContestRepository _contestRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Cloudinary _cloudinary;
    private readonly IConfiguration _configuration;


    public ContestService(IContestRepository contestRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
      _contestRepository = contestRepository ?? throw new ArgumentNullException(nameof(contestRepository));
      _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

      // Configure Cloudinary
      // var cloudinaryAccount = new Account(
      //   _configuration["Cloudinary:CloudName"],
      //   _configuration["Cloudinary:ApiKey"],
      //   _configuration["Cloudinary:ApiSecret"]
      // );
      // System.Console.WriteLine("Cloudinary account: " + cloudinaryAccount.ApiKey);
      // System.Console.WriteLine("Cloudinary account: " + cloudinaryAccount.ApiSecret);
      // System.Console.WriteLine("Cloudinary account: " + cloudinaryAccount.Cloud);
      // _cloudinary = new Cloudinary(cloudinaryAccount);
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
        // ImageUrl = await UploadImageToCloudinary(model.ImageUrl)
        ImageUrl = model.ImageUrl
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
        RuleDescription = model.RuleDescription,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        MinimumParticipant = model.MinimumParticipant,
        MaximumParticipant = model.MaximumParticipant,
        Prizes = model.Prizes,
        ParticipantInformationRequirements = model.ParticipantInformationRequirements,
        CreatorUserId = userId,
        OrganizationInformation = model.OrganizationInformation,
        // ImageUrl = await UploadImageToCloudinary(model.ImageUrl)
        ImageUrl = model.ImageUrl
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

    private async Task<string> UploadImageToCloudinary(string imageData)
    {
      var uploadParams = new ImageUploadParams()
      {
        File = new FileDescription("contest_image.jpg", new MemoryStream(Convert.FromBase64String(imageData))),
        PublicId = "contests/" + Guid.NewGuid().ToString(),
        Transformation = new Transformation().Width(800).Height(600).Crop("fill")
      };
      var uploadResult = await _cloudinary.UploadAsync(uploadParams);
      return uploadResult.SecureUrl.ToString();
    }
  }
}