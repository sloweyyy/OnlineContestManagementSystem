namespace OnlineContestManagement.Models
{
    public class RegistrationStatisticsModel
    {
        public int RegistrationsToday { get; set; }
        public int RegistrationsYesterday { get; set; }
        public double RegistrationsGrowthPercentage { get; set; }
    }
}
