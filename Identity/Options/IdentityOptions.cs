namespace Identity.Options
{
    public class IdentityOptions
    {
        public IdentityRoutes Routes { get; }
            = new IdentityRoutes();

        public IdentityFeatures Features { get; }
            = new IdentityFeatures();
    }

    public class IdentityRoutes
    {
        public string RefreshRoute { get; set; } = null;
    }

    public class IdentityFeatures
    {
        public bool IsAvailableRefreshToken { get; set; } = true;
    }
}