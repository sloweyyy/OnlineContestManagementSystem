// File: OnlineContestManagement/Data/Models/User.cs
using Microsoft.AspNetCore.Identity;

namespace OnlineContestManagement.Data.Models
{
    public class User : IdentityUser
    {
        public String FullName { get; set; }
        public String Role { get; set; }
        
    }
}