using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
  public class PrizeModel
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public string ImageUrl { get; set; }
    public decimal Amount { get; set; }
  }
}