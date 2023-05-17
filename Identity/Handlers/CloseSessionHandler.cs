using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class CloseSessionHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Delete;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }
        public bool ProtectedRoute { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            //ToDo will be implemented later
            return (APIResponse.OK(), 200);
        }

        public CloseSessionHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.CloseSessionRoute;
            IsAvailable = identityOptions.Features.IsAvailableSessions;
            ProtectedRoute = true;
        }
    }
}
