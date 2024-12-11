using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineContestManagement.Infrastructure.Services;
using OnlineContestManagement.Models;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class NewsController : ControllerBase
  {
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
      _newsService = newsService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateNews([FromBody] CreateNewsRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var news = new News
      {
        Name = request.Name,
        ImageUrl = request.ImageUrl,
        CreatedAt = DateTime.UtcNow
      };

      var createdNews = await _newsService.CreateNewsAsync(news);
      return Ok(createdNews);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsById(string id)
    {
      var news = await _newsService.GetNewsByIdAsync(id);
      if (news == null)
      {
        return NotFound();
      }
      return Ok(news);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllNews()
    {
      var newsList = await _newsService.GetAllNewsAsync();
      return Ok(newsList);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNews(string id, [FromBody] UpdateNewsRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var news = new News
      {
        Id = id,
        Name = request.Name,
        ImageUrl = request.ImageUrl
      };

      var updatedNews = await _newsService.UpdateNewsAsync(id, news);
      if (updatedNews == null)
      {
        return NotFound();
      }
      return Ok(updatedNews);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNews(string id)
    {
      await _newsService.DeleteNewsAsync(id);
      return Ok(new { Message = "News deleted successfully" });
    }
  }
}