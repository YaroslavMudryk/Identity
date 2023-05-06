using Identity.Models;

namespace Identity.Services.Interfaces
{
    public interface ILocationService
    {
        Task<LocationInfo> GetIpInfoAsync(string ip);
    }
}
