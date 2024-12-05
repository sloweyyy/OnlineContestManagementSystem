namespace OnlineContestManagement.Models
{
    public class RegistrationStatisticsResponse
    {
        public int RegistrationsToday { get; set; }
        public int RegistrationsYesterday { get; set; }
        public double RegistrationsGrowthPercentage { get; set; }
    }
}
