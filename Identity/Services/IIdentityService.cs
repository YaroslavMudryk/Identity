namespace Identity.Services
{
    public interface IIdentityService
    {
        Task<string> GetCurrentTokenAsync();
    }
}
