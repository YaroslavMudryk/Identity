using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class DisplayRoutesHandler : IHandler
    {
        private readonly IdentityRoutes _routes;

        public string Method { get; set; } = HttpMethods.Get;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            return (new APIResponse
            {
                Ok = true,
                Error = null,
                Data = _routes
            }, 200);
        }

        public DisplayRoutesHandler(IdentityOptions identityOptions)
        {
            Route = "/identity/routes";
            IsAvailable = identityOptions.Routes.IsAvailableToDisplayRoutes;
            _routes = identityOptions.Routes;
        }
    }
}
