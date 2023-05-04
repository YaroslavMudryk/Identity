using Identity.Options;
using Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, Action<IdentityOptions> configureOptions)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IIdentityService, IdentityService>();
            services.Configure(configureOptions);
            return services;
        }
    }
}
