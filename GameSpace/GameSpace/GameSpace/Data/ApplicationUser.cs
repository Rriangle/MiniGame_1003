using System;
using Microsoft.AspNetCore.Identity;

namespace GameSpace.Data
{
    /// <summary>
    /// Custom identity user with additional profile metadata used by the MiniGame admin area.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public string? UserStatus { get; set; }

        public string? User_Address { get; set; }

        public DateTime? User_birthdate { get; set; }

        public string? User_email { get; set; }

        public string? User_phone { get; set; }

        public DateTime? User_registration_date { get; set; }

        public DateTime? User_CreatedAt { get; set; }
    }
}
