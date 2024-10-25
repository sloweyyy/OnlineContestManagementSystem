using Microsoft.AspNetCore.Identity;
using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IUserService
    {
        Task<string> GenerateJwtTokenAsync(User user);
    }
}