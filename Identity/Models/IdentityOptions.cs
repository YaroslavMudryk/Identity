namespace Identity.Models
{
    public class IdentityOptions
    {
        public bool IsAvailableRefresh { get; set; } = true;
        public string RefreshPath { get; set; }
    }
}