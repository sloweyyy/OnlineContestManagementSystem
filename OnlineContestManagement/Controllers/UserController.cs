using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null) return BadRequest("User data cannot be null.");

            var result = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            if (id != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updateUserBody)
        {
            if (id != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Forbid();

            if (updateUserBody == null) return BadRequest("Update data cannot be null.");

            var result = await _userService.UpdateUserAsync(id, updateUserBody);
            if (!result.Success) return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Success) return StatusCode(result.StatusCode, result.Message);

            return NoContent();
        }
    }
}
