using Microsoft.AspNetCore.Identity;
using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
    }
}