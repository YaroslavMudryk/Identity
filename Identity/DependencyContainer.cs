﻿using Identity.Db.Context;
using Identity.Db.Providers;
using Identity.Options;
using Identity.Services;
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
            services.AddScoped<IIdentityService, IdentityService>();
            services.Configure(configureOptions);
            return services;
        }

        private static void CheckAndSetupConnection(DbContextOptionsBuilder options, IdentityDatabase db)
        {
            var prov = db.Privider;
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
