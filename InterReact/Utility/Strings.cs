using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace InterReact;

public static partial class Xtensions
{
    extension(IEnumerable<string> strings)
    {
        public string JoinStrings(string separator = "") => string.Join(separator, strings);
    }

    extension(object instance)
    {
        public string Stringify(bool includeTypeName = true)
        {
            string str = JsonSerializer.Serialize(instance, _options);
            Type type = instance.GetType();
            if (includeTypeName)
                return $"{type.Name} {str}";
            else
                return str;
        }
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
