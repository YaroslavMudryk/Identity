using Identity.Extensions;
using Identity.Models.Response;
using Identity.Options;
using Identity.Seeder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Handlers
{
    public class SeedSystemHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Post;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }
        public bool ProtectedRoute { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            var seederService = httpContext.RequestServices.GetRequiredService<ISeederService>();

            var resposne = await seederService.SeedSystemAsync();

            return resposne.MapToResponse();
        }

        public SeedSystemHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.SeedSystemRoute;
            IsAvailable = identityOptions.Features.IsAvailableSeedSystem;
            ProtectedRoute = false;
        }
    }
}
