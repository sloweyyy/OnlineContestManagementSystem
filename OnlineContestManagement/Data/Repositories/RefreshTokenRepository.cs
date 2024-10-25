using OnlineContestManagement.Data.Models;
using MongoDB.Driver;

namespace OnlineContestManagement.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;

        public RefreshTokenRepository(IMongoDatabase database)
        {
            _refreshTokenCollection = database.GetCollection<RefreshToken>("RefreshTokens");
        }

        public async Task CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _refreshTokenCollection.InsertOneAsync(refreshToken);
        }

        public async Task<RefreshToken> GetRefreshTokenByTokenAsync(string token)
        {
            return await _refreshTokenCollection.Find(x => x.Token == token).FirstOrDefaultAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, token);
            var update = Builders<RefreshToken>.Update.Set(x => x.IsRevoked, true);
            await _refreshTokenCollection.UpdateOneAsync(filter, update);
        }
    }
}