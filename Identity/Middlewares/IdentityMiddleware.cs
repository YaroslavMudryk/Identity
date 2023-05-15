using Identity.Extensions;
using Identity.Handlers;
using Identity.Helpers;
using Identity.Options;
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

        private readonly List<IHandler> _handlers;

        private readonly RequestDelegate _next;

        public IdentityMiddleware(RequestDelegate next, IOptions<IdentityOptions> options)
        {
            _next = next;
            _options = options.Value;
            _routes = options.Value.Routes;
            _features = options.Value.Features;
            _account = options.Value.Account;

            _handlers = options.Value.BuildHandlers();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            foreach (var handler in _handlers)
            {
                if (handler.CanHandle(httpContext))
                {
                    var resposne = await handler.HandleAsync(httpContext);

                    httpContext.Response.ContentType = "application/json; charset=utf-8";
                    httpContext.Response.StatusCode = resposne.Item2;
                    await httpContext.Response.WriteAsync(resposne.Item1.ToJson());
                    return;
                }
            }

            await _next.Invoke(httpContext);
        }
    }
}
