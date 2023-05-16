namespace Identity.Constants
{
    public class EndpointsConstants
    {
        public static string SignupEndpoint { get; set; } = "/api/v1/identity/sign-up";
        public static string LoginEndpoint { get; set; } = "/api/v1/identity/sign-in";
        public static string Login2MfaEndpoint { get; set; } = "/api/v1/identity/sign-in/2mfa";
        public static string RefreshEndpoint { get; set; } = "/api/v1/identity/refresh";
        public static string LogoutEndpoint { get; set; } = "/api/v1/identity/log-out";
        public static string SendConfirmEndpoint { get; set; } = "/api/v1/identity/send-confirm";
        public static string ConfirmEndpoint { get; set; } = "/api/v1/identity/confirm";
        public static string EnableMfaEndpoint { get; set; } = "/api/v1/identity/2mfa";
        public static string DisableMfaEndpoint { get; set; } = "/api/v1/identity/2mfa/{0}";
        public static string ChangePasswordEndpoint { get; set; } = "/api/v1/identity/change-password";
        public static string ChangeLoginEndpoint { get; set; } = "/api/v1/identity/change-login";
        public static string SessionsEndpoint { get; set; } = "/api/v1/identity/sessions";
        public static string CloseSessionEndpoint { get; set; } = "/api/v1/identity/sessions/{0}";
    }
}
