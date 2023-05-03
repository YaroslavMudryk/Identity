using Identity.Constants;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Identity.Middlewares
{
    public class IdentityMiddleware
    {
        private readonly IdentityOptions _options;
        private readonly IIdentityService _identityService;
        private readonly RequestDelegate _next;

        public IdentityMiddleware(RequestDelegate next, /*IdentityOptions options,*/ IIdentityService identityService)
        {
            _next = next;
            _options = new IdentityOptions
            {
                IsAvailableRefresh = true,
                RefreshPath = EndpointsConstants.RefreshEndpoint
            };
            _identityService = identityService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (_options.IsAvailableRefresh && httpContext.Request.Path == _options.RefreshPath)
            {
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync("Success");
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }

    public static class IdentityMiddlewareExtensions
    {
        public static void UseIdentityHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<IdentityMiddleware>();
        }
    }
}
