using Identity.Extensions;
using Identity.Models.Response;
using Identity.Options;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Handlers
{
    public class RetrieveSessionsHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Get;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }
        public bool ProtectedRoute { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            var sessionService = httpContext.RequestServices.GetRequiredService<ISessionService>();

            var isActive = httpContext.Request.Query["isActive"].FirstOrDefault();
            var page = httpContext.Request.Query["page"].FirstOrDefault();

            var result = await sessionService.GetUserSessionsAsync(isActive.ToBool(true), page.ToInt(1));

            return result.MapToResponse();
        }

        public RetrieveSessionsHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.SessionsRoute;
            IsAvailable = identityOptions.Features.IsAvailableSessions;
            ProtectedRoute = true;
        }
    }
}
