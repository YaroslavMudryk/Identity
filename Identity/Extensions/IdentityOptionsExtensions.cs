using Identity.Handlers;
using Identity.Helpers;
using Identity.Options;
using System.Reflection;

namespace Identity.Extensions
{
    public static class IdentityOptionsExtensions
    {
        public static List<IHandler> BuildHandlers(this IdentityOptions identityOptions)
        {
            var handlerTypes = ClassFinder.FindClassesImplementingInterface<IHandler>();

            var handlers = new List<IHandler>();

            foreach (var handler in handlerTypes)
            {
                handlers.Add((IHandler)Activator.CreateInstance(handler, identityOptions));
            }

            return handlers;
        }
    }
}
