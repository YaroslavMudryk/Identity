using Identity.Db;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models;

public class UserLogin : BaseModel<int>
{
    public string Key { get; set; }
    public string Provider { get; set; }
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}