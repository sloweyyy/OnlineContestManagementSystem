using MongoDB.Driver;
using OnlineContestManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Data.Repositories
{
  public class NewsRepository : INewsRepository
  {
    private readonly IMongoCollection<News> _newsCollection;

    public NewsRepository(IMongoDatabase database)
    {
      _newsCollection = database.GetCollection<News>("News");
    }

    public async Task CreateNewsAsync(News news)
    {
      await _newsCollection.InsertOneAsync(news);
    }

    public async Task<News> GetNewsByIdAsync(string id)
    {
      return await _newsCollection.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<News>> GetAllNewsAsync()
    {
      return await _newsCollection.Find(Builders<News>.Filter.Empty).ToListAsync();
    }

    public async Task UpdateNewsAsync(string id, News news)
    {
      var filter = Builders<News>.Filter.Eq(n => n.Id, id);
      await _newsCollection.ReplaceOneAsync(filter, news);
    }

    public async Task DeleteNewsAsync(string id)
    {
      var filter = Builders<News>.Filter.Eq(n => n.Id, id);
      await _newsCollection.DeleteOneAsync(filter);
    }
  }
}