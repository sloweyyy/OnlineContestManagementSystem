using Microsoft.AspNetCore.Identity;
using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(string userId);
        Task<(bool Success, string Message, int StatusCode)> UpdateUserAsync(string id, User updateUserBody);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<(bool Success, string Message, int StatusCode)> DeleteUserAsync(string id);
    }
}