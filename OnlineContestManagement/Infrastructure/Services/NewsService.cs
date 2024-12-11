using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Infrastructure.Services
{
  public class NewsService : INewsService
  {
    private readonly INewsRepository _newsRepository;

    public NewsService(INewsRepository newsRepository)
    {
      _newsRepository = newsRepository;
    }

    public async Task<News> CreateNewsAsync(News news)
    {
      news.CreatedAt = DateTime.UtcNow;
      await _newsRepository.CreateNewsAsync(news);
      return news;
    }

    public async Task<News> GetNewsByIdAsync(string id)
    {
      return await _newsRepository.GetNewsByIdAsync(id);
    }

    public async Task<List<News>> GetAllNewsAsync()
    {
      return await _newsRepository.GetAllNewsAsync();
    }

    public async Task<News> UpdateNewsAsync(string id, News news)
    {
      await _newsRepository.UpdateNewsAsync(id, news);
      return news;
    }

    public async Task DeleteNewsAsync(string id)
    {
      await _newsRepository.DeleteNewsAsync(id);
    }
  }
}