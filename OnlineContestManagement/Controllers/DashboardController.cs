using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("contest-statistics")]
        public async Task<IActionResult> GetContestStatistics()
        {
            var statistics = await _dashboardService.GetContestStatisticsAsync();
            return Ok(statistics);
        }

        [HttpGet("registration-statistics")]
        public async Task<IActionResult> GetRegistrationStatistics()
        {
            var statistics = await _dashboardService.GetRegistrationStatisticsAsync();
            return Ok(statistics);
        }

        [HttpGet("total-contests")]
        public async Task<IActionResult> GetTotalContests()
        {
            int totalContests = await _dashboardService.GetTotalContestsAsync();
            return Ok(totalContests);
        }

        [HttpGet("contest-participants")]
        public async Task<IActionResult> GetContestParticipants()
        {
            var participants = await _dashboardService.GetContestParticipantsAsync();
            return Ok(participants);
        }

        [HttpGet("contest-revenue")]
        public async Task<IActionResult> GetContestRevenue()
        {
            var contestRevenue = await _dashboardService.GetContestRevenueAsync();
            return Ok(new { ContestRevenue = contestRevenue });
        }

        [HttpGet("website-revenue")]
        public async Task<IActionResult> GetWebsiteRevenue()
        {
            var websiteRevenue = await _dashboardService.GetWebsiteRevenueAsync();
            return Ok(new { WebsiteRevenue = websiteRevenue });
        }

        [HttpGet("total-participants")]
        public async Task<IActionResult> GetTotalParticipants()
        {
            int totalParticipants = await _dashboardService.GetTotalParticipantsAsync();
            return Ok(totalParticipants);
        }

    }
}
