using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class ChangePasswordHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Post;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            //ToDo will be implemented later
            return (new APIResponse(), 200);
        }

        public ChangePasswordHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.ChangePasswordRoute;
            IsAvailable = identityOptions.Features.IsAvailableChangeLoginAndPassword;
        }
    }
}
