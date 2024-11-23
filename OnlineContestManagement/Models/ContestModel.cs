using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
  public class CreateContestModel
  {
    [Required]
    public string Name { get; set; }

    [Required]
    public string RuleDescription { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public int MinimumParticipant { get; set; }

    [Required]
    public int MaximumParticipant { get; set; }

    [Required]
    public List<PrizeModel> Prizes { get; set; }

    [Required]
    public List<string> ParticipantInformationRequirements { get; set; }

    [Required]
    public OrganizationInformationModel OrganizationInformation { get; set; }

    [Required]
    public string ImageUrl { get; set; }
    [Required]
    public decimal EntryFee { get; set; }
  }

  public class UpdateContestModel
  {
    public string Name { get; set; }

    public string RuleDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MinimumParticipant { get; set; }

    public int MaximumParticipant { get; set; }

    public List<PrizeModel> Prizes { get; set; }

    public List<string> ParticipantInformationRequirements { get; set; }

    public OrganizationInformationModel OrganizationInformation { get; set; }

    public string ImageUrl { get; set; }
    public decimal EntryFee { get; set; }
  }

  public class ContestSearchFilter
  {
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; } // "Open", "Closed", "Ongoing"
  }
}