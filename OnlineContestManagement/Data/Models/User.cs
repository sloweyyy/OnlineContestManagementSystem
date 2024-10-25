using Microsoft.AspNetCore.Identity;

namespace OnlineContestManagement.Data.Models
{
    public class User : IdentityUser
    {
        public String FullName { get; set; }
        public String Role { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}