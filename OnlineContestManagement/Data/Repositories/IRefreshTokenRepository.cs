using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenByTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
    }
}