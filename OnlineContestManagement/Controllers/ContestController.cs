using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContestController : BaseController
    {
        private readonly IContestService _contestService;

        public ContestController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContest([FromBody] Contest contest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _contestService.CreateContestAsync(contest);
            return Ok(new { Message = "Tạo cuộc thi thành công" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContestById(string id)
        {
            var contest = await _contestService.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }

            return Ok(contest);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContests()
        {
            var contests = await _contestService.GetAllContestsAsync();
            return Ok(contests);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContest(string id, [FromBody] Contest contest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingContest = await _contestService.GetContestByIdAsync(id);
            if (existingContest == null)
            {
                return NotFound();
            }

            contest.Id = id;
            await _contestService.UpdateContestAsync(contest);
            return Ok(new { Message = "Cập nhật cuộc thi thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContest(string id)
        {
            var existingContest = await _contestService.GetContestByIdAsync(id);
            if (existingContest == null)
            {
                return NotFound();
            }

            await _contestService.DeleteContestAsync(id);
            return Ok(new { Message = "Xóa cuộc thi thành công" });
        }
    }
}
