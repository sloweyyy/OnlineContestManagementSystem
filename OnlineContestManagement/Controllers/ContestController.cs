using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestsController : ControllerBase
    {
        private readonly IContestService _contestService;

        public ContestsController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContest([FromBody] Contest contest)
        {
            await _contestService.CreateContestAsync(contest);
            return CreatedAtAction(nameof(GetContestById), new { id = contest.Id }, contest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContestById(string id)
        {
            var contest = await _contestService.GetContestByIdAsync(id);
            return contest == null ? NotFound() : Ok(contest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContest(string id, [FromBody] Contest contest)
        {
            contest.Id = id;
            await _contestService.UpdateContestAsync(contest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContest(string id)
        {
            await _contestService.DeleteContestAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchContests([FromQuery] string keyword, [FromQuery] int? minParticipants, [FromQuery] int? maxParticipants, [FromQuery] List<string> skills)
        {
            var contests = await _contestService.SearchContestsAsync(keyword, minParticipants, maxParticipants, skills);
            return Ok(contests);
        }
    }
}
