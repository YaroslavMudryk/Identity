using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class Login2MfaHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Post;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            //ToDo will be implemented later
            return (APIResponse.OK(), 200);
        }

        public Login2MfaHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.Login2MfaRoute;
            IsAvailable = identityOptions.Features.IsAvailableMfa;
        }
    }
}
