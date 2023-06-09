﻿using Identity.Db;
using System.ComponentModel.DataAnnotations;
namespace Identity.Models
{
    public class RoleClaim : BaseModel<int>
    {
        public int ClaimId { get; set; }
        public Claim Claim { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

    }
}
