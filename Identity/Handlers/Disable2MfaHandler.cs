using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class Disable2MfaHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Post;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }
        public bool ProtectedRoute { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            //ToDo will be implemented later
            return (APIResponse.OK(), 200);
        }

        public Disable2MfaHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.DisableMfaRoute;
            IsAvailable = identityOptions.Features.IsAvailableMfa;
            ProtectedRoute = true;
        }
    }
}
