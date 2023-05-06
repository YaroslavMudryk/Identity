using Identity.Helpers;
using Identity.Options;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Identity.Services.Implementations
{
    public class HttpIdentityService : IIdentityService
    {
        private readonly HttpContext _httpContext;
        public HttpIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string GetUserId()
        {
            if (!IsAuth())
                return null;
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == ConstantsClaimTypes.UserId);
            return claim.Value;
        }

        public string GetIP()
        {
            return _httpContext.Connection.RemoteIpAddress.ToString();
        }

        public bool IsAdmin()
        {
            if (!IsAuth())
                return false;
            return GetRoles().Any(s => s.Contains(DefaultsRoles.Administrator));
        }

        public IEnumerable<string> GetRoles()
        {
            if (!IsAuth())
                return Enumerable.Empty<string>();
            var claims = _httpContext.User.Claims.Where(s => s.Type == ConstantsClaimTypes.Role);
            return claims.Select(x => x.Value);
        }

        private bool IsAuth()
        {
            return _httpContext.User.Identity.IsAuthenticated;
        }

        public Guid GetCurrentSessionId()
        {
            if (!IsAuth())
                return Guid.Empty;
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == ConstantsClaimTypes.SessionId);
            return Guid.Parse(claim.Value);
        }

        public string GetBearerToken()
        {
            var bearerWord = "Bearer ";
            var bearerToken = _httpContext.Request.Headers["Authorization"].ToString();
            if (bearerToken.StartsWith(bearerWord, StringComparison.OrdinalIgnoreCase))
            {
                return bearerToken.Substring(bearerWord.Length).Trim();
            }
            return bearerToken;
        }
    }
}