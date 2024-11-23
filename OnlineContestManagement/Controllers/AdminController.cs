using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;

namespace OnlineContestManagement.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize(Roles = "Admin")]
  public class AdminController : ControllerBase
  {
    private readonly IContestService _contestService;

    public AdminController(IContestService contestService)
    {
      _contestService = contestService;
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveContest(string id)
    {
      try
      {
        await _contestService.ApproveContestAsync(id);
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectContest(string id)
    {
      try
      {
        await _contestService.RejectContestAsync(id);
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  }
}