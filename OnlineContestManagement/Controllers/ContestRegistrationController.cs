using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/contests/{contestId}/registration")]
    [Authorize]
    public class ContestRegistrationController : ControllerBase
    {
        private readonly IContestRegistrationRepository _registrationRepository;
        private readonly IEmailService _emailService;
        private IContestRegistrationService _registrationService;

        public ContestRegistrationController(IContestRegistrationRepository registrationRepository, IEmailService emailService)
        {
            _registrationRepository = registrationRepository;
            _emailService = emailService;
        }

        public async Task<bool> RegisterUserForContestAsync(RegisterForContestModel registrationModel)
        {
            var registration = new ContestRegistration
            {
                ContestId = registrationModel.ContestId,
                UserId = registrationModel.UserId,
                Name = registrationModel.Name,
                DateOfBirth = registrationModel.DateOfBirth,
                Email = registrationModel.Email,
                AdditionalInfo = registrationModel.AdditionalInfo,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            var result = await _registrationRepository.RegisterUserAsync(registration);
            if (result)
            {
                await _emailService.SendRegistrationConfirmation(registrationModel.Email, registrationModel.ContestId);
            }
            return result;
        }

        public ContestRegistrationController(IContestRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForContest(string contestId, [FromBody] RegisterForContestModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            registrationModel.ContestId = contestId; // Gán ContestId từ route

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
            withdrawModel.ContestId = contestId; // Gán ContestId từ route

            var result = await _registrationService.WithdrawUserFromContestAsync(withdrawModel);
            if (result)
            {
                return Ok(new { Message = "Withdrawal successful." });
            }
            return BadRequest(new { Message = "Withdrawal failed." });
        }
    }
}
