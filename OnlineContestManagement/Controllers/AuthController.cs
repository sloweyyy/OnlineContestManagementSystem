using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IUserService userService, IAuthService authService, IEmailService emailService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.RegisterUserAsync(model);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "User registration failed", Errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "User registration failed", Error = ex.Message });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _authService.SignInAsync(model);
                return Ok(response);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Sign-in failed", Error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(response);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Token refresh failed", Error = ex.Message });
            }
        }


        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
                return Ok(new { Message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Token revocation failed", Error = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> GenerateResetPasswordToken([FromBody] ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resetToken = await _authService.GenerateResetPasswordTokenAsync(model.Email);
                await _emailService.SendResetPasswordEmail(model.Email, resetToken);
                return Ok(new { Message = "Reset password token generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to generate reset password token", Error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.ResetPasswordAsync(model.ResetToken, model.NewPassword);
                if (result)
                {
                    return Ok(new { Message = "Password reset successfully" });
                }
                return BadRequest(new { Message = "Password reset failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Password reset failed", Error = ex.Message });
            }
        }

    }
}