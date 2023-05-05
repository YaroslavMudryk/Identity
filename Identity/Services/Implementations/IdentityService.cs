using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
namespace Identity.Services.Implementations
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpContext _context;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor.HttpContext;
        }
        public async Task<string> GetCurrentTokenAsync()
        {
            return await _context.GetTokenAsync("access_token");
        }
    }
}