using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace InterReact
{
    public static class Utilities
    {
        // return an instance of a subclass of T which constructed with the single inputParameter.
        internal static T? TryConstructSubclassTaking<T>(object inputParameter)
        {
            Type[] inputParameterTypes = new[] { inputParameter.GetType() };

            ConstructorInfo? ctor = typeof(T).Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(T)))
                .Where(t => t.IsPublic && !t.IsAbstract)
                .Select(t => t.GetConstructor(inputParameterTypes))
                .Where(c => c != null)
                .Cast<ConstructorInfo>()
                .SingleOrDefault();

            if (ctor == default)
                return default;

            object instance = ctor.Invoke(new object[] { inputParameter });

            return (T)instance;
        }
    }
}
