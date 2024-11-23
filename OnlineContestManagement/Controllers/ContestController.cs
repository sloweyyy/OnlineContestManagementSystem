using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class ContestController : BaseController
  {
    private readonly IContestService _contestService;

    public ContestController(IContestService contestService)
    {
      _contestService = contestService ?? throw new ArgumentNullException(nameof(contestService));
    }

    [HttpPost]
    public async Task<IActionResult> CreateContest([FromBody] CreateContestModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var contest = await _contestService.CreateContestAsync(model);
        return Ok(contest);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Contest creation failed", Error = ex.Message });
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContestDetails(string id)
    {
      try
      {
        var contest = await _contestService.GetContestDetailsAsync(id);
        if (contest == null)
        {
          return NotFound();
        }
        return Ok(contest);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Error retrieving contest details", Error = ex.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContests()
    {
      try
      {
        List<Contest> contests;
        if (User.IsInRole("Admin"))
        {
          contests = await _contestService.GetAllContestsAsync();
        }
        else
        {
          var allContests = await _contestService.GetAllContestsAsync();
          contests = allContests.Where(c => c.Status == "approved").ToList();
        }
        return Ok(contests);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Error retrieving all contests", Error = ex.Message });
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContest(string id, [FromBody] UpdateContestModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var updatedContest = await _contestService.UpdateContestAsync(id, model);
        if (updatedContest == null)
        {
          return NotFound();
        }
        return Ok(updatedContest);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Contest update failed", Error = ex.Message });
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContest(string id)
    {
      try
      {
        await _contestService.DeleteContestAsync(id);
        return Ok(new { Message = "Contest deleted successfully" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Contest deletion failed", Error = ex.Message });
      }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchContests([FromQuery] ContestSearchFilter filter)
    {
      try
      {
        var contests = await _contestService.SearchContestsAsync(filter);
        return Ok(contests);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Error searching contests", Error = ex.Message });
      }
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<IActionResult> GetContestsByCreatorId(string creatorId)
    {
      if (!User.IsInRole("Admin") && User.FindFirstValue(ClaimTypes.NameIdentifier) != creatorId)
      {
        return Forbid();
      }

      try
      {
        var contests = await _contestService.GetContestsByCreatorIdAsync(creatorId);
        return Ok(contests);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Error retrieving contests by creator", Error = ex.Message });
      }
    }
  }
}