using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenRepository _tokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IOptions<JwtSettings> jwtSettings,
            ITokenRepository tokenRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
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
                return IdentityResult.Failed(new IdentityError { Code = "EmailExists", Description = "Email already exists" });
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

        public async Task<AuthResponse> SignInAsync(SignInModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(model.Email)) throw new ArgumentException("Email cannot be empty", nameof(model.Email));
            if (string.IsNullOrEmpty(model.Password)) throw new ArgumentException("Password cannot be empty", nameof(model.Password));

            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                throw new AuthenticationException("Invalid email or password");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                throw new AuthenticationException("Invalid email or password");
            }

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(refreshToken, user);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role
                }
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            var existingRefreshToken = await _tokenRepository.GetTokenByValueAsync(refreshToken, TokenType.RefreshToken);
            if (existingRefreshToken == null)
            {
                throw new AuthenticationException("Invalid refresh token");
            }

            if (existingRefreshToken.IsRevoked)
            {
                throw new AuthenticationException("Refresh token is revoked");
            }

            if (existingRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new AuthenticationException("Refresh token has expired");
            }

            var user = await _userRepository.GetUserByIdAsync(existingRefreshToken.UserId);
            if (user == null)
            {
                throw new AuthenticationException("User not found");
            }

            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(newRefreshToken, user);
            await RevokeRefreshTokenAsync(refreshToken);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            var existingRefreshToken = await _tokenRepository.GetTokenByValueAsync(refreshToken, TokenType.RefreshToken);
            if (existingRefreshToken == null)
            {
                throw new AuthenticationException("Invalid refresh token");
            }

            if (existingRefreshToken.IsRevoked)
            {
                throw new AuthenticationException("Refresh token is already revoked");
            }

            await _tokenRepository.RevokeTokenAsync(refreshToken, TokenType.RefreshToken);
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
                throw new InvalidOperationException("JWT secret key is not configured");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task SaveRefreshTokenAsync(string refreshToken, User user)
        {
            var token = new Token
            {
                TokenValue = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpiryDays),
                Type = TokenType.RefreshToken
            };

            await _tokenRepository.CreateTokenAsync(token);
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

        public async Task<string> GenerateResetPasswordTokenAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var resetToken = GenerateResetPasswordToken();
            await SaveResetPasswordTokenAsync(resetToken, user);
            return resetToken;
        }

        public async Task<bool> ResetPasswordAsync(string resetToken, string newPassword)
        {
            var existingToken = await _tokenRepository.GetTokenByValueAsync(resetToken, TokenType.ResetPassword);
            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiryDate < DateTime.UtcNow)
            {
                throw new ArgumentException("Invalid or expired reset token");
            }

            var user = await _userRepository.GetUserByIdAsync(existingToken.UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateUserAsync(user);
            await _tokenRepository.RevokeTokenAsync(resetToken, TokenType.ResetPassword);

            return true;
        }

        private string GenerateResetPasswordToken()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task SaveResetPasswordTokenAsync(string resetToken, User user)
        {
            var token = new Token
            {
                TokenValue = resetToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddHours(1),
                Type = TokenType.ResetPassword
            };

            await _tokenRepository.CreateTokenAsync(token);
        }
    }
}