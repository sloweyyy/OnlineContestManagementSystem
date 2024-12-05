using OnlineContestManagement.Data.Models;
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
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IContestRepository contestRepository, IContestRegistrationRepository registrationRepository, IPaymentRepository paymentRepository, ILogger<DashboardService> logger)
        {
            _contestRepository = contestRepository;
            _registrationRepository = registrationRepository;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<ContestStatisticsResponse> GetContestStatisticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var contestsToday = await _contestRepository.CountContestsByDateAsync(today);
            var contestsYesterday = await _contestRepository.CountContestsByDateAsync(yesterday);
            var contestsGrowth = CalculateGrowthPercentage(contestsToday, contestsYesterday);

            return new ContestStatisticsResponse
            {
                ContestsToday = contestsToday,
                ContestsYesterday = contestsYesterday,
                ContestsGrowthPercentage = contestsGrowth
            };
        }

        public async Task<RegistrationStatisticsResponse> GetRegistrationStatisticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var registrationsToday = await _registrationRepository.CountRegistrationsByDateAsync(today);
            var registrationsYesterday = await _registrationRepository.CountRegistrationsByDateAsync(yesterday);
            var registrationsGrowth = CalculateGrowthPercentage(registrationsToday, registrationsYesterday);

            return new RegistrationStatisticsResponse
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

        public async Task<int> GetTotalContestsAsync()
        {
            return await _contestRepository.GetTotalContestsAsync();
        }

        public async Task<Dictionary<string, List<ContestRegistration>>> GetContestParticipantsAsync()
        {
            return await _registrationRepository.GetContestParticipantsAsync();
        }

        public async Task<decimal> GetContestRevenueAsync()
        {
            return await _paymentRepository.GetTotalRevenueAsync();
        }

        public async Task<RevenueStatisticsResponse> GetWebsiteRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var todayRevenue = await _paymentRepository.GetTotalRevenueByDateAsync(today);
            var yesterdayRevenue = await _paymentRepository.GetTotalRevenueByDateAsync(yesterday);
            var growthPercentage = CalculateGrowthPercentage(todayRevenue, yesterdayRevenue);

            return new RevenueStatisticsResponse
            {
                TodayRevenue = todayRevenue * 0.3m,
                YesterdayRevenue = yesterdayRevenue * 0.3m,
                GrowthPercentage = growthPercentage
            };
        }

        private double CalculateGrowthPercentage(decimal todayCount, decimal yesterdayCount)
        {
            if (yesterdayCount == 0)
            {
                return todayCount > 0 ? 100 : 0;
            }
            return ((double)(todayCount - yesterdayCount) / (double)yesterdayCount) * 100;
        }


        public async Task<int> GetTotalParticipantsAsync()
        {
            return await _registrationRepository.GetTotalParticipantsAsync();
        }

        public async Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync()
        {
            var revenueData = await _paymentRepository.GetMonthlyRevenueAsync();
            var currentYear = DateTime.UtcNow.Year;
            var lastYear = currentYear - 1;

            var monthlyRevenue = Enumerable.Range(1, 12).Select(month => new MonthlyRevenueResponse
            {
                Month = $"Tháng {month}",
                LastYear = revenueData
                    .Where(r => r._id.Year == lastYear && r._id.Month == month)
                    .Sum(r => r.TotalRevenue),
                ThisYear = revenueData
                    .Where(r => r._id.Year == currentYear && r._id.Month == month)
                    .Sum(r => r.TotalRevenue)
            }).ToList();

            return monthlyRevenue;
        }

        public async Task<List<FeaturedContest>> GetFeaturedContestsAsync(int topN = 5)
        {
            return await _registrationRepository.GetFeaturedContestsAsync(topN);
        }

        public async Task<List<QuarterlyContestDataResponse>> GetQuarterlyContestDataAsync()
        {
            var statusCounts = await _contestRepository.GetQuarterlyContestCountsAsync();

            var data = statusCounts
                .GroupBy(q => new { q.Year, q.Quarter })
                .Select(g => new QuarterlyContestDataResponse
                {
                    Quarter = $"{g.Key.Year} {g.Key.Quarter}",
                    OnBoarding = g.Where(x => x.Status == "Đang diễn ra").Sum(x => x.ContestCount),
                    ComingSoon = g.Where(x => x.Status == "Sắp diễn ra").Sum(x => x.ContestCount),
                    Ended = g.Where(x => x.Status == "Đã kết thúc").Sum(x => x.ContestCount),
                })
                .ToList();

            _logger.LogInformation("Quarterly contest data transformed successfully.");

            return data;
        }
    }
}
