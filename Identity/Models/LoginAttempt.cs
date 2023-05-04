using Extensions.DeviceDetector.Models;
using Identity.Db;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models;

public class LoginAttempt : BaseModel<int>
{
    public string Login { get; set; }
    public string Password { get; set; }
    public ClientInfo Device { get; set; }
    public LocationInfo Location { get; set; }
    public bool IsSuccess { get; set; }
    public int? UserId { get; set; }
    public User User { get; set; }
}