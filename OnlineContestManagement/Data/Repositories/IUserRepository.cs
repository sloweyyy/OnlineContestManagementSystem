using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(string userId);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}