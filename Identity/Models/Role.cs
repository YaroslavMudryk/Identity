﻿using Identity.Db;
using System.ComponentModel.DataAnnotations;
namespace Identity.Models
{
    public class Role : BaseModel<int>
    {
        [Required, StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }
        [Required, StringLength(150, MinimumLength = 1)]
        public string NameNormalized { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<RoleClaim> RoleClaims { get; set; }

        public Role(string name)
        {
            Name = name;
            NameNormalized = name.ToUpper();
        }
        public Role()
        {

        }
    }
}
