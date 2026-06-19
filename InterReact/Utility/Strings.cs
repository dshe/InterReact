using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace InterReact;

public static class Strings
{
    public static string JoinStrings(this IEnumerable<string> strings, string separator = "") => string.Join(separator, strings);

    public static string Stringify<T>(this T instance, bool includeTypeName = true)
    {
        var tx = JsonSerializer.Serialize(instance, _options);
        if (includeTypeName)
            return $"{typeof(T).Name}: {tx}";
        else
            return tx;
    }

    private static readonly JsonSerializerOptions _options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(typeInfo =>
        {
            foreach (JsonPropertyInfo property in typeInfo.Properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var original = property.ShouldSerialize;
                    property.ShouldSerialize = (obj, value) =>
                    {
                        if (value is string s && s.Length == 0)
                            return false;
                        return original?.Invoke(obj, value) ?? true;
                    };
                }
            }
        });

        return new JsonSerializerOptions
        {
            //WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            TypeInfoResolver = resolver,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
    }
}
