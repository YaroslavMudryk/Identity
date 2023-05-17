using Extensions.Password;
using Identity.Db.Context;
using Identity.Helpers;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Seeder
{
    public class BaseSeederService : ISeederService
    {
        private readonly IdentityContext _db;

        public BaseSeederService(IdentityContext db)
        {
            _db = db;
        }

        public virtual async Task<Result<bool>> SeedSystemAsync()
        {
            int counter = 0;

            if (!_db.Roles.Any())
            {
                await _db.Roles.AddRangeAsync(GetDefaultsRoles());
                counter++;
            }
            if (!_db.Apps.Any())
            {
                await _db.Apps.AddRangeAsync(GetDefaultApps());
                counter++;
            }

            if (counter > 0)
            {
                await _db.SaveChangesAsync();
                await SetupDefaultUserAsync();
                return Result<bool>.Success();
            }
            return Result<bool>.Error("Identity db have data");
        }

        private IEnumerable<Role> GetDefaultsRoles()
        {
            yield return new Role
            {
                Name = DefaultsRoles.Administrator,
                NameNormalized = DefaultsRoles.Administrator.ToUpper()
            };

            yield return new Role
            {
                Name = DefaultsRoles.User,
                NameNormalized = DefaultsRoles.User.ToUpper()
            };
        }

        private IEnumerable<App> GetDefaultApps()
        {
            var today = DateTime.Today;

            yield return new App
            {
                Name = "Web app",
                Description = "Base web application",
                IsActive = true,
                ShortName = "Web",
                ActiveFrom = today,
                ActiveTo = today.AddYears(5),
                ClientId = "F94A4E87C1FD23C102800D",
                ClientSecret = "b2e459a6c58a472da53e47a46d6c5ad4c1ecda073aa4402b9724ac55cd65dcc4"
            };
        }

        private async Task SetupDefaultUserAsync()
        {
            var passwordHash = "Admin01!".GeneratePasswordHash();
            var newUser = new User
            {
                AccessFailedCount = 0,
                Confirms = null,
                Email = "admin@mail.com",
                FirstName = "Admin",
                Image = null,
                IsConfirmed = true,
                LastName = "Admin",
                LockoutEnabled = false,
                LockoutEnd = null,
                Login = "admin@mail.com",
                LoginAttempts = null,
                Logins = null,
                MFA = false,
                MFAs = null,
                MFASecretKey = null,
                Name = "Admin Admin",
                PasswordHash = passwordHash,
                Passwords = new List<Password> { new Password { Answer = "Some", Question = "Question", IsActive = true, PasswordHash = passwordHash } },
                Phone = "0501539538",
                Qrs = null,
                Sessions = null,
                UserLogins = null,
                UserName = "Admin"
            };

            var adminRole = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.NameNormalized == DefaultsRoles.Administrator.ToUpper());

            var newUserRole = new UserRole
            {
                User = newUser,
                RoleId = adminRole.Id,
                IsActive = true,
                ActiveFrom = DateTime.Now,
                ActiveTo = DateTime.Now.AddYears(10),
            };

            if (!_db.Users.Any() && !_db.UserRoles.Any())
            {
                await _db.Users.AddAsync(newUser);
                await _db.UserRoles.AddAsync(newUserRole);
                await _db.SaveChangesAsync();
            }
        }
    }
}
