using Identity.Helpers;
using Identity.Options;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Identity.Middlewares
{
    public class IdentityMiddleware
    {
        private readonly IdentityOptions _options;
        private readonly IdentityFeatures _features;
        private readonly IdentityRoutes _routes;
        private readonly IdentityAccount _account;

        private readonly RequestDelegate _next;

        public IdentityMiddleware(RequestDelegate next, IOptions<IdentityOptions> options)
        {
            _next = next;
            _options = options.Value;
            _routes = options.Value.Routes;
            _features = options.Value.Features;
            _account = options.Value.Account;
        }

        public async Task InvokeAsync(HttpContext httpContext, IIdentityService identityService, IAppService appService)
        {
            if (_routes.IsAvailableToDisplayRoutes && httpContext.Request.Path == "/identity/routes" && httpContext.Request.Method == HttpMethods.Get)
            {
                httpContext.Response.ContentType = "application/json; charset=utf-8";
                await httpContext.Response.WriteAsync(_routes.ToJson());
                return;
            }
            if (_features.IsAvailableRefreshToken && httpContext.Request.Path == _routes.RefreshRoute && httpContext.Request.Method == HttpMethods.Post)
            {

                // handle refresh token
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync("success");
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
