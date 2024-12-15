using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public interface ITokenRepository
    {
        Task CreateTokenAsync(Token refreshToken);
        Task<Token> GetTokenByValueAsync(string token, TokenType type);
        Task RevokeTokenAsync(string token, TokenType type);
    }
}