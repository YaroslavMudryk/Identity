using Identity.Helpers;
using Identity.ViewModels;

namespace Identity.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<List<SessionViewModel>>> GetUserSessionsAsync(bool isActive, int page);
        Task<Result<int>> CloseSessionsByIdsAsync(Guid[] ids);
    }
}
