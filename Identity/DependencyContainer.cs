using Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
