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
    }
}
