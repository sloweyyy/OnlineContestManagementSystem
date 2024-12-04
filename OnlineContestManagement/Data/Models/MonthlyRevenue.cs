namespace OnlineContestManagement.Data.Models
{
  public class MonthlyRevenue
  {
    public RevenueId _id { get; set; }
    public decimal TotalRevenue { get; set; }
  }

  public class RevenueId
  {
    public int Year { get; set; }
    public int Month { get; set; }
  }
}