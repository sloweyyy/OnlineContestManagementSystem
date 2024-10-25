using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(2)]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; } = "User";
        
    }
}