using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class RetrieveSessionsHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Get;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            //ToDo will be implemented later
            return (new APIResponse(), 200);
        }

        public RetrieveSessionsHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.SessionsRoute;
            IsAvailable = identityOptions.Features.IsAvailableSessions;
        }
    }
}
