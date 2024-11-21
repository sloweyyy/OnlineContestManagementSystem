using Microsoft.AspNetCore.Identity;

namespace OnlineContestManagement.Data.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }
}