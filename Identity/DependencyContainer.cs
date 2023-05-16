using Extensions.DeviceDetector;
using Identity.Db.Context;
using Identity.Db.Providers;
using Identity.Options;
using Identity.Services.Implementations;
using Identity.Services.Interfaces;
using Identity.SessionHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, Action<IdentityOptions> configureOptions, IdentityDatabase dbSettings)
        {
            services.AddDbContext<IdentityContext>(options =>
            {
                CheckAndSetupConnection(options,dbSettings);
            });

            services.AddHttpContextAccessor();
            services.AddDeviceDetector();
            services.AddSingleton<ISessionManager, SessionManager>();

            services.AddScoped<IAppService, AppService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IIdentityService, HttpIdentityService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ITokenService, TokenService>();

            services.Configure(configureOptions);
            return services;
        }

        private static void CheckAndSetupConnection(DbContextOptionsBuilder options, IdentityDatabase db)
        {
            var prov = db.Provider;
            if (prov is SqlServerProvider)
                options.UseSqlServer(db.ConnectionString);
            if (prov is MySqlProvider)
                options.UseMySql(db.ConnectionString, ServerVersion.AutoDetect(db.ConnectionString));
            if (prov is PostgreSqlProvider)
                options.UseNpgsql(db.ConnectionString);
            if (prov is SqliteProvider)
                options.UseSqlite(db.ConnectionString);
        }
    }
}
