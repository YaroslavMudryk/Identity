﻿using Identity.Constants;
using Identity.Db.Providers;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        public string RegisterRoute { get; set; } = EndpointsConstants.SignupEndpoint;
        public string LoginRoute { get; set; } = EndpointsConstants.LoginEndpoint;
        public string Login2MfaRoute { get; set; } = EndpointsConstants.Login2MfaEndpoint;
        public string RefreshRoute { get; set; } = EndpointsConstants.RefreshEndpoint;
        public string LogoutRoute { get; set; } = EndpointsConstants.LogoutEndpoint;
        public string SendConfirmRoute { get; set; } = EndpointsConstants.SendConfirmEndpoint;
        public string ConfirmRoute { get; set; } = EndpointsConstants.ConfirmEndpoint;
        public string EnableMfaRoute { get; set; } = EndpointsConstants.EnableMfaEndpoint;
        public string DisableMfaRoute { get; set; } = EndpointsConstants.DisableMfaEndpoint;
        public string RestorePasswordRoute { get; set; } = EndpointsConstants.RestorePasswordEndpoint;
        public string ChangeLoginRoute { get; set; } = EndpointsConstants.ChangeLoginEndpoint;
        public string ChangePasswordRoute { get; set; } = EndpointsConstants.ChangePasswordEndpoint;
        public string SessionsRoute { get; set; } = EndpointsConstants.SessionsEndpoint;
        public string CloseSessionRoute { get; set; } = EndpointsConstants.CloseSessionEndpoint;
    }

    public class IdentityFeatures
    {
        public bool IsAvailableRefreshToken { get; set; } = true;
        public bool IsAvailableConfirm { get; set; } = true;
        public bool IsAvailableMfa { get; set; } = true;
        public bool IsAvailableRestorePassword { get; set; } = true;
        public bool IsAvailableSessions { get; set; } = true;
        public bool IsAvailableChangeLoginAndPassword { get; set; } = true;
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