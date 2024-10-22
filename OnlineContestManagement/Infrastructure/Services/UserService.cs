using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(model.Password)) throw new ArgumentException("Password cannot be empty", nameof(model.Password));
            if (string.IsNullOrEmpty(model.Email)) throw new ArgumentException("Email cannot be empty", nameof(model.Email));

            if (!IsValidEmail(model.Email))
                throw new ArgumentException("Invalid email format", nameof(model.Email));

            var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "EmailExists"
                ,Description = "Email already exists" });
            }


            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Role = model.Role 
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            try
            {
                await _userRepository.CreateUserAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                throw new Exception("User creation failed", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }

}