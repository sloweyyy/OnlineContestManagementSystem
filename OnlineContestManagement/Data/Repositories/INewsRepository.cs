using OnlineContestManagement.Models;

namespace OnlineContestManagement.Data.Repositories
{
  public interface INewsRepository
  {
    Task CreateNewsAsync(News news);
    Task<News> GetNewsByIdAsync(string id);
    Task<List<News>> GetAllNewsAsync();
    Task UpdateNewsAsync(string id, News news);
    Task DeleteNewsAsync(string id);
  }
}