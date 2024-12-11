using OnlineContestManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Infrastructure.Services
{
  public interface INewsService
  {
    Task<News> CreateNewsAsync(News news);
    Task<News> GetNewsByIdAsync(string id);
    Task<List<News>> GetAllNewsAsync();
    Task<News> UpdateNewsAsync(string id, News news);
    Task DeleteNewsAsync(string id);
  }
}