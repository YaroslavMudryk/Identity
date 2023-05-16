using Extensions.DeviceDetector.Models;
using System.ComponentModel.DataAnnotations;

namespace Identity.Dtos
{
    public class LoginDto
    {
        [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Lang { get; set; }
        public ClientInfo Device { get; set; }
        public AppLoginDto App { get; set; }
    }
}
