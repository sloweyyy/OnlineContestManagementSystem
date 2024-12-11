using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
  public class CreateNewsRequest
  {
    [Required]
    public string Name { get; set; }

    [Required]
    public string ImageUrl { get; set; }
  }

  public class UpdateNewsRequest
  {
    [Required]
    public string Name { get; set; }

    [Required]
    public string ImageUrl { get; set; }
  }

}