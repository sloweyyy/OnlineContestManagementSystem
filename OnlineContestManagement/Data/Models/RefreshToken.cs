using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Data.Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}