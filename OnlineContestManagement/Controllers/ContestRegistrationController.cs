using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/contests")]
    [Authorize]
    public class ContestRegistrationController : ControllerBase
    {
        private readonly IContestRegistrationRepository _registrationRepository;
        private readonly IEmailService _emailService;
        private readonly IContestRegistrationService _registrationService;

        public ContestRegistrationController(
            IContestRegistrationRepository registrationRepository,
            IEmailService emailService,
            IContestRegistrationService registrationService)
        {
            _registrationRepository = registrationRepository;
            _emailService = emailService;
            _registrationService = registrationService;
        }

        [HttpPost("{contestId}")]
        public async Task<IActionResult> RegisterForContest(string contestId, [FromBody] RegisterForContestModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            registrationModel.ContestId = contestId;
            registrationModel.UserId = userId;

            var result = await _registrationService.RegisterUserForContestAsync(registrationModel);
            if (result)
            {
                return Ok(new { Message = "Registration successful." });
            }
            return BadRequest(new { Message = "Registration failed." });
        }
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFromContest(string contestId, [FromBody] WithdrawFromContestModel withdrawModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            withdrawModel.ContestId = contestId;

            var result = await _registrationService.WithdrawUserFromContestAsync(withdrawModel);
            if (result)
            {
                return Ok(new { Message = "Withdrawal successful." });
            }
            return BadRequest(new { Message = "Withdrawal failed." });
        }

        [HttpGet("registration/user/{userId}")]
        public async Task<IActionResult> GetContestsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "UserId is required." });
            }

            var registrations = await _registrationService.GetContestsByUserIdAsync(userId);
            if (registrations == null || registrations.Count == 0)
            {
                return NotFound(new { Message = "No registrations found for this user." });
            }

            return Ok(registrations);
        }
    }
}
