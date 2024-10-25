using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
        Task<AuthResponse> SignInAsync(SignInModel model);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}