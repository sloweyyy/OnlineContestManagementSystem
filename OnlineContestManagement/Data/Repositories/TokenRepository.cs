using OnlineContestManagement.Data.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace OnlineContestManagement.Data.Repositories
{
  public class TokenRepository : ITokenRepository
  {
    private readonly IMongoCollection<Token> _tokenCollection;

    public TokenRepository(IMongoDatabase database)
    {
      _tokenCollection = database.GetCollection<Token>("Tokens");
    }

    public async Task CreateTokenAsync(Token token)
    {
      await _tokenCollection.InsertOneAsync(token);
    }

    public async Task<Token> GetTokenByValueAsync(string tokenValue, TokenType type)
    {
      return await _tokenCollection.Find(x => x.TokenValue == tokenValue && x.Type == type).FirstOrDefaultAsync();
    }

    public async Task RevokeTokenAsync(string tokenValue, TokenType type)
    {
      var filter = Builders<Token>.Filter.And(
          Builders<Token>.Filter.Eq(x => x.TokenValue, tokenValue),
          Builders<Token>.Filter.Eq(x => x.Type, type)
      );
      var update = Builders<Token>.Update.Set(x => x.IsRevoked, true);
      await _tokenCollection.UpdateOneAsync(filter, update);
    }
  }
}