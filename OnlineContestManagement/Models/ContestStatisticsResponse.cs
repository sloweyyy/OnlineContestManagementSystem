namespace OnlineContestManagement.Models
{
    public class ContestStatisticsResponse
    {
        public int ContestsToday { get; set; }
        public int ContestsYesterday { get; set; }
        public double ContestsGrowthPercentage { get; set; }
    }
}
