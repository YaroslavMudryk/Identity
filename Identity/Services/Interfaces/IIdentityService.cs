namespace Identity.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetCurrentTokenAsync();
    }
}
