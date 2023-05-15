using Identity.Extensions;
using Identity.SessionHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Identity.Middlewares
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISessionManager _sessionManager;
        public SessionMiddleware(RequestDelegate next, ISessionManager sessionManager)
        {
            _next = next;
            _sessionManager = sessionManager;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var token = await httpContext.GetTokenAsync("access_token");
            if (httpContext.IsAuthenticationRequired())
            {
                if (token == null || !_sessionManager.IsActiveSession(token))
                {
                    httpContext.Response.StatusCode = 401;
                    await httpContext.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            await _next(httpContext);
        }
    }
}
