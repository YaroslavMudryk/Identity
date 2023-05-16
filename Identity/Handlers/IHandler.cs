using Identity.Extensions;
using Identity.Models.Response;
using Microsoft.AspNetCore.Http;

namespace Identity.Handlers
{
    public interface IHandler
    {
        string Method { get; set; }
        string Route { get; set; }
        public bool IsAvailable { get; set; }
        public bool ProtectedRoute { get; set; }
        bool CanHandle(HttpContext httpContext)
        {
            return IsAvailable && httpContext.Request.Method == Method && httpContext.Request.Path.IsValidPath(Route);
        }
        Task<(APIResponse, int)> HandleAsync(HttpContext httpContext);
    }
}
