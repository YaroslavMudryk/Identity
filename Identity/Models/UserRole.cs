﻿using Extensions.DeviceDetector.Models;
using Identity.Db;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class UserRole : BaseModel<int>
    {
        public bool IsActive { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
