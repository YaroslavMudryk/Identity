﻿using Identity.Db;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class User : BaseModel<int>
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        [Required]
        public int AccessFailedCount { get; set; }
        [Required]
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool IsConfirmed { get; set; }
        public bool MFA { get; set; }
        public string MFASecretKey { get; set; }
        public List<Session> Sessions { get; set; }
        public List<Password> Passwords { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Qr> Qrs { get; set; }
        public List<Confirm> Confirms { get; set; }
        public List<UserLogin> UserLogins { get; set; }
        public List<LoginAttempt> LoginAttempts { get; set; }
        public List<MFA> MFAs { get; set; }
        public List<LoginChange> Logins { get; set; }


        public bool IsLocked()
        {
            var isLocked = false;
            if (LockoutEnd == null)
                isLocked = false;
            if (LockoutEnd.HasValue)
            {
                if (LockoutEnd.Value > DateTime.Now)
                    isLocked = true;
                else
                {
                    LockoutEnd = null;
                    isLocked = false;
                }
            }
            return isLocked;
        }
    }
}