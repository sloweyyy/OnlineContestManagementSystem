using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("contest-statistics")]
        public async Task<IActionResult> GetContestStatistics()
        {
            var statistics = await _dashboardService.GetContestStatisticsAsync();
            return Ok(statistics);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("registration-statistics")]
        public async Task<IActionResult> GetRegistrationStatistics()
        {
            var statistics = await _dashboardService.GetRegistrationStatisticsAsync();
            return Ok(statistics);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("total-contests")]
        public async Task<IActionResult> GetTotalContests()
        {
            int totalContests = await _dashboardService.GetTotalContestsAsync();
            return Ok(totalContests);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("contest-participants")]
        public async Task<IActionResult> GetContestParticipants()
        {
            var participants = await _dashboardService.GetContestParticipantsAsync();
            return Ok(participants);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("contest-revenue")]
        public async Task<IActionResult> GetContestRevenue()
        {
            var contestRevenue = await _dashboardService.GetContestRevenueAsync();
            return Ok(new { ContestRevenue = contestRevenue });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("website-revenue")]
        public async Task<IActionResult> GetWebsiteRevenue()
        {
            var websiteRevenue = await _dashboardService.GetWebsiteRevenueAsync();
            return Ok(websiteRevenue);
        }

        [HttpGet("total-participants")]
        public async Task<IActionResult> GetTotalParticipants()
        {
            int totalParticipants = await _dashboardService.GetTotalParticipantsAsync();
            return Ok(totalParticipants);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var monthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync();
            return Ok(monthlyRevenue);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("featured-contests")]
        public async Task<IActionResult> GetFeaturedContests()
        {
            var featuredContests = await _dashboardService.GetFeaturedContestsAsync();
            return Ok(featuredContests);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("quarterly-contest-counts")]
        public async Task<IActionResult> GetQuarterlyContestCounts()
        {
            var quarterlyCounts = await _dashboardService.GetQuarterlyContestDataAsync();
            return Ok(quarterlyCounts);
        }

        [HttpGet("comingsoon-contests")]
        public async Task<IActionResult> GetComingSoonContestCount()
        {
            int count = await _dashboardService.GetComingSoonContestCountAsync();
            return Ok(new { ComingSoonContests = count });
        }

        [HttpGet("onboarding-contests")]
        public async Task<IActionResult> GetOnBoardingContestCount()
        {
            int count = await _dashboardService.GetOnBoardingContestCountAsync();
            return Ok(new { OnBoardingContests = count });
        }



    }
}
