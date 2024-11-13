using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
    public class RegisterForContestModel
    {
        [Required]
        public string ContestId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public Dictionary<string, string> AdditionalInfo { get; set; }
    }

    public class WithdrawFromContestModel
    {
        [Required]
        public string ContestId { get; set; }

        [Required]
        public string UserId { get; set; }
    }

    public class ContestRegistrationSearchFilter
    {
        public string? ContestId { get; set; }
        public string? UserId { get; set; }
        public string? Status { get; set; }
    }
}
