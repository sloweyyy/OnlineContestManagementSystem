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
        private readonly IContestService _contestService;

        public ContestRegistrationController(
            IContestRegistrationRepository registrationRepository,
            IEmailService emailService,
            IContestRegistrationService registrationService,
            IContestService contestService)
        {
            _registrationRepository = registrationRepository;
            _emailService = emailService;
            _registrationService = registrationService;
            _contestService = contestService;
        }

        [HttpPost("{contestId}")]
        public async Task<IActionResult> RegisterForContest(string contestId, [FromBody] RegisterForContestModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            registrationModel.UserId = userId;

            var result = await _registrationService.RegisterUserForContestAsync(contestId, registrationModel);

            if (result.Success)
            {
                return Ok(new { Message = result.Message, PaymentLink = result.PaymentLink });
            }

            return BadRequest(new { Message = result.Error });
        }
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFromContest([FromBody] WithdrawFromContestModel withdrawModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


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

            var result = registrations.Select(async r => new
            {
                r.ContestId,
                ContestDetails = await _contestService.GetContestDetailsAsync(r.ContestId),
                r.RegistrationDate,
                r.Status
            }).ToList();


            return Ok(result);
        }

        [HttpGet("{contestId}/registrations")]
        public async Task<IActionResult> GetRegistrationsByContestId(string contestId)
        {
            var registrations = await _registrationService.GetRegistrationsByContestIdAsync(contestId);
            if (registrations == null || registrations.Count == 0)
            {
                return NotFound(new { Message = "No registrations found for this contest." });
            }
            return Ok(registrations);
        }
    }
}
