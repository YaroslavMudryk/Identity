namespace Identity.Services.Interfaces
{
    public interface IIdentityService
    {
        int GetUserId();
        string GetIP();
        bool IsAdmin();
        Guid GetCurrentSessionId();
        IEnumerable<string> GetRoles();
        string GetBearerToken();
    }
}