using Identity.Middlewares;
using Identity.Options;
using Microsoft.AspNetCore.Builder;

namespace Identity
{
    public static class IdentityApplicationBuilderExtensions
    {
        public static void UseIdentityHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<IdentityMiddleware>();
        }

        public static void UseIdentityHandler(this IApplicationBuilder builder, IdentityOptions options)
        {
            builder.UseMiddleware<IdentityMiddleware>(Microsoft.Extensions.Options.Options.Create(options));
        }
    }
}
