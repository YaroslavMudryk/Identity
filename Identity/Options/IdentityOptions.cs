using Identity.Constants;
using Identity.Db.Providers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace Identity.Options
{
    public class IdentityOptions
    {
        public JwtTokenOptions Token { get; }
            = new JwtTokenOptions();
        public IdentityRoutes Routes { get; }
            = new IdentityRoutes();

        public IdentityFeatures Features { get; }
            = new IdentityFeatures();

        public IdentityAccount Account { get; }
            = new IdentityAccount();
    }

    public class IdentityRoutes
    {
        public bool IsAvailableToDisplayRoutes { get; set; } = true;
        public string RegisterRoute { get; set; } = EndpointsConstants.SignupEndpoint;
        public string LoginRoute { get; set; } = EndpointsConstants.LoginEndpoint;
        public string Login2MfaRoute { get; set; } = EndpointsConstants.Login2MfaEndpoint;
        public string RefreshRoute { get; set; } = EndpointsConstants.RefreshEndpoint;
        public string LogoutRoute { get; set; } = EndpointsConstants.LogoutEndpoint;
        public string SendConfirmRoute { get; set; } = EndpointsConstants.SendConfirmEndpoint;
        public string ConfirmRoute { get; set; } = EndpointsConstants.ConfirmEndpoint;
        public string EnableMfaRoute { get; set; } = EndpointsConstants.EnableMfaEndpoint;
        public string DisableMfaRoute { get; set; } = EndpointsConstants.DisableMfaEndpoint;
        public string ChangeLoginRoute { get; set; } = EndpointsConstants.ChangeLoginEndpoint;
        public string ChangePasswordRoute { get; set; } = EndpointsConstants.ChangePasswordEndpoint;
        public string SessionsRoute { get; set; } = EndpointsConstants.SessionsEndpoint;
        public string CloseSessionRoute { get; set; } = EndpointsConstants.CloseSessionEndpoint;
        public string SeedSystemRoute { get; set; } = EndpointsConstants.SeedSystemEndpoint;
    }

    public class IdentityFeatures
    {
        public bool IsAvailableRefreshToken { get; set; } = true;
        public bool IsAvailableConfirm { get; set; } = true;
        public bool IsAvailableMfa { get; set; } = true;
        public bool IsAvailableRestorePassword { get; set; } = true;
        public bool IsAvailableSessions { get; set; } = true;
        public bool IsAvailableChangeLoginAndPassword { get; set; } = true;
        public bool IsAvailableSeedSystem { get; set; } = true;
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
        public string RegexTemplate { get; set; } = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
        public string RegexErrorMessage { get; set; } = "Invalid password";
    }

    public class IdentityDatabase
    {
        public string ConnectionString { get; set; }
        public IDbProvider Provider { get; set; }
    }

    public class JwtTokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int LifeTimeInMinutes { get; set; }


        private string SecretKey = "49f1df0cefdc05744f90b3fdc246ecc7dd372ca3016e22418b66e632f1744622";
        public SymmetricSecurityKey GetSymmetricSecurityKey()
            => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }
}