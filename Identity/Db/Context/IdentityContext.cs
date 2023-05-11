using Identity.Db.Helpers;
using Identity.Models;
using Identity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Db.Context
{
    public class IdentityContext : DbContext
    {
        private readonly IIdentityService _identityService;
        public IdentityContext(DbContextOptions<IdentityContext> options, IIdentityService identityService) : base(options)
        {
            _identityService = identityService;
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

        public override int SaveChanges()
        {
            this.ApplyAuditInfo(_identityService);

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfo(_identityService);

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
