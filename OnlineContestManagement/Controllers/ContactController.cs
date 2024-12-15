using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ContactController : ControllerBase
  {
    private readonly IEmailService _emailService;

    public ContactController(IEmailService emailService)
    {
      _emailService = emailService;
    }

    [HttpPost("submit")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitContactForm([FromBody] ContactFormModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        await _emailService.SendContactFormEmail(model);
        return Ok(new { Message = "Contact form submitted successfully" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Failed to submit contact form", Error = ex.Message });
      }
    }
  }
}