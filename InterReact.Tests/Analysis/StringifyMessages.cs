using System.Text.Json.Serialization.Metadata;
using System.Reflection;
namespace Analysis;

public class StringifyMessages(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public void Stringify_Messages()
    {
        DefaultJsonTypeInfoResolver resolver = new();
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

        Type[] types = [.. typeof(InterReactClient)
            .Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<MessageAttribute>() != null)
            .OrderBy(x => x.Name)];

        foreach (TypeInfo type in types)
        {
            try
            {
                var instance = Activator.CreateInstance(type, nonPublic: true);
                string str = instance.Stringify();
                Write($"{type.Name} = {str}");
            }
            catch (Exception e)
            {
                Write($"Type: {type.Name} EXCEPTION: {e.Message}");
                throw;
            }
        }
    }
}
