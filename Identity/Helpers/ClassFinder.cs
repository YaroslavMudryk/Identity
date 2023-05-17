using System.Reflection;

namespace Identity.Helpers
{
    public class ClassFinder
    {
        public static IEnumerable<Type> FindClassesImplementingInterface<T>()
        {
            var interfaceType = typeof(T);
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetTypes()
                .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface);
        }
    }
}
