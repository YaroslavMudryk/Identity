using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Dtos
{
    public class AppClaimsDto
    {
        public int Id { get; set; }
        public int[] ClaimIds { get; set; }
    }
}
