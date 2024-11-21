namespace OnlineContestManagement.Models
{
    public class ContestStatisticsModel
    {
        public int ContestsToday { get; set; }
        public int ContestsYesterday { get; set; }
        public double ContestsGrowthPercentage { get; set; }
    }
}
