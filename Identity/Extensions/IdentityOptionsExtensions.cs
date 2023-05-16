using Identity.Handlers;
using Identity.Options;

namespace Identity.Extensions
{
    public static class IdentityOptionsExtensions
    {
        public static List<IHandler> BuildHandlers(this IdentityOptions identityOptions)
        {
            var handlers = new List<IHandler>();

            handlers.Add(new DisplayRoutesHandler(identityOptions));

            handlers.Add(new RegisterHandler(identityOptions));
            handlers.Add(new LoginHandler(identityOptions));
            handlers.Add(new Login2MfaHandler(identityOptions));
            handlers.Add(new RefreshTokenHandler(identityOptions));
            handlers.Add(new LogoutHandler(identityOptions));
            handlers.Add(new SendConfirmHandler(identityOptions));
            handlers.Add(new ConfirmHandler(identityOptions));
            handlers.Add(new Enable2MfaHandler(identityOptions));
            handlers.Add(new Disable2MfaHandler(identityOptions));
            handlers.Add(new ChangeLoginHandler(identityOptions));
            handlers.Add(new ChangePasswordHandler(identityOptions));
            handlers.Add(new RetrieveSessionsHandler(identityOptions));
            handlers.Add(new CloseSessionHandler(identityOptions));

            return handlers;
        }
    }
}
