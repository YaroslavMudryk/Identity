using Identity.Constants;
using Identity.Db.Providers;

namespace Identity.Options
{
    public class IdentityOptions
    {
        public IdentityRoutes Routes { get; }
            = new IdentityRoutes();

        public IdentityFeatures Features { get; }
            = new IdentityFeatures();

        public IdentityAccount Account { get; }
            = new IdentityAccount();
    }

    public class IdentityRoutes
    {
        public string RefreshRoute { get; set; } = EndpointsConstants.RefreshEndpoint;
    }

    public class IdentityFeatures
    {
        public bool IsAvailableRefreshToken { get; set; } = true;
    }

    public class IdentityAccount
    {
        public IdentityBlocking Blocking { get; }
            = new IdentityBlocking();

        public IdentityPassword Password { get; }
            = new IdentityPassword();
    }

    public class IdentityBlocking
    {
        public int AfterFailedAttempts { get; set; } = 5;
        public TimeSpan TimeFirstBlock { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan TimeNextBlock { get; set; } = TimeSpan.FromHours(2);
        public bool LogoutOtherSessions { get; set; } = false;
    }

    public class IdentityPassword
    {
        public int MinSize { get; set; } = 4;
        public int MaxSize { get; set; } = 25;
        public bool MandatoryCapitalCharecter { get; set; } = true;
        public bool MandatoryNumbers { get; set; } = true;
        public bool AllowPartOfEmail { get; set; } = false;
        public string AllowSpecialCharacters { get; set; } = ".,/?|%$#*&_";
    }

    public class IdentityDatabase
    {
        public string ConnectionString { get; set; }
        public IDbProvider Privider { get; set; }
    }
}