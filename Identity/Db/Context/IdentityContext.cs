using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Db.Context
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<AppClaim> AppClaims { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<LoginChange> LoginChanges { get; set; }
        public DbSet<Qr> Qrs { get; set; }
        public DbSet<Confirm> Confirms { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<MFA> MFAs { get; set; }
    }
}
