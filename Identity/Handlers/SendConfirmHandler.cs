using Identity.Models.Response;
using Identity.Options;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public class SendConfirmHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Post;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            //ToDo will be implemented later
            return (new APIResponse(), 200);
        }

        public SendConfirmHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.SendConfirmRoute;
            IsAvailable = identityOptions.Features.IsAvailableConfirm;
        }
    }
}
