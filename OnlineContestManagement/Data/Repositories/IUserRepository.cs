using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(string userId);
    }
}