using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;


        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task<User> CreateUserAsync(User user)
        {
            await _userRepository.CreateUserAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<(bool Success, string Message, int StatusCode)> UpdateUserAsync(string id, User updateUserBody)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return (false, "User not found.", 404);

            if (!string.IsNullOrWhiteSpace(updateUserBody.Email))
                user.Email = updateUserBody.Email;

            if (!string.IsNullOrWhiteSpace(updateUserBody.FullName))
                user.FullName = updateUserBody.FullName;

            if (!string.IsNullOrWhiteSpace(updateUserBody.PhoneNumber))
                user.PhoneNumber = updateUserBody.PhoneNumber;

            try
            {
                await _userRepository.UpdateUserAsync(user);
                return (true, "User updated successfully.", 200);
            }
            catch (Exception ex)
            {
                return (false, $"Internal server error: {ex.Message}", 500);
            }
        }

        public async Task<(bool Success, string Message, int StatusCode)> DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return (false, "User not found.", 404);

            await _userRepository.DeleteUserAsync(id);
            return (true, "User deleted successfully.", 204);
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }


    }
}