using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System;
using System.Threading.Tasks;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IContestRepository _contestRepository;
        private readonly IContestRegistrationRepository _registrationRepository;

        public DashboardService(IContestRepository contestRepository, IContestRegistrationRepository registrationRepository)
        {
            _contestRepository = contestRepository;
            _registrationRepository = registrationRepository;
        }

        public async Task<ContestStatisticsModel> GetContestStatisticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var contestsToday = await _contestRepository.CountContestsByDateAsync(today);
            var contestsYesterday = await _contestRepository.CountContestsByDateAsync(yesterday);
            var contestsGrowth = CalculateGrowthPercentage(contestsToday, contestsYesterday);

            return new ContestStatisticsModel
            {
                ContestsToday = contestsToday,
                ContestsYesterday = contestsYesterday,
                ContestsGrowthPercentage = contestsGrowth
            };
        }

        public async Task<RegistrationStatisticsModel> GetRegistrationStatisticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var registrationsToday = await _registrationRepository.CountRegistrationsByDateAsync(today);
            var registrationsYesterday = await _registrationRepository.CountRegistrationsByDateAsync(yesterday);
            var registrationsGrowth = CalculateGrowthPercentage(registrationsToday, registrationsYesterday);

            return new RegistrationStatisticsModel
            {
                RegistrationsToday = registrationsToday,
                RegistrationsYesterday = registrationsYesterday,
                RegistrationsGrowthPercentage = registrationsGrowth
            };
        }

        private double CalculateGrowthPercentage(int todayCount, int yesterdayCount)
        {
            if (yesterdayCount == 0)
            {
                return todayCount > 0 ? 100 : 0;
            }
            return ((double)(todayCount - yesterdayCount) / yesterdayCount) * 100;
        }
    }
}
