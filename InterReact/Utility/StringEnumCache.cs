using System.Linq.Expressions;
using System.Reflection;
namespace InterReact;

// Generic Static Cache !
// codes are case-sensitive

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000: Do not declare static members on generic types", Justification = "Intentional per-closed-generic cache")]
internal static class StringEnumCache<T> where T : class  
{
    internal static IReadOnlyDictionary<string, T> Values { get; } = Build();
    internal static Func<string, T> Factory { get; } = CreateFactory();
    private static Dictionary<string, T> Build()
    {
        Type type = typeof(T);

        PropertyInfo codeProperty = type.GetProperty("Code") ??
            throw new InvalidOperationException($"Type '{type.Name}' does not expose a Code property.");
        if (codeProperty.PropertyType != typeof(string))
            throw new InvalidOperationException($"Type '{type.Name}', 'Code' property is not a string.");

        return type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsInitOnly && f.FieldType == type) // readonly
            .Select(f => (T)f.GetValue(null)!)
            .ToDictionary(x => (string)codeProperty.GetValue(x)!, x => x);
        // duplicate codes will throw
    }

    private static Func<string, T> CreateFactory()
    {
        ConstructorInfo ctor = typeof(T).GetConstructor([typeof(string)])
            ?? throw new InvalidOperationException();
        ParameterExpression p = Expression.Parameter(typeof(string), "code");
        NewExpression body = Expression.New(ctor, p);
        return Expression.Lambda<Func<string, T>>(body, p).Compile();
    }

}
