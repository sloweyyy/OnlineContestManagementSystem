using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
    public class SignInModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        public string ResetToken { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }

}