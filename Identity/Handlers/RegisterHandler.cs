using Identity.Dtos;
using Identity.Extensions;
using Identity.Models.Response;
using Identity.Options;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Handlers
{
    public class RegisterHandler : IHandler
    {
        public string Method { get; set; } = HttpMethods.Post;
        public string Route { get; set; }
        public bool IsAvailable { get; set; }
        public bool ProtectedRoute { get; set; }

        public async Task<(APIResponse, int)> HandleAsync(HttpContext httpContext)
        {
            var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();

            var registerDto = await httpContext.Request.Body.GetBodyAsync<RegisterDto>();

            var result = await authService.RegisterAsync(registerDto);

            return result.MapToResponse();
        }

        public RegisterHandler(IdentityOptions identityOptions)
        {
            Route = identityOptions.Routes.RegisterRoute;
            IsAvailable = true;
            ProtectedRoute = false;
        }
    }
}
