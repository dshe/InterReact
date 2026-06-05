using System.Text.Json.Serialization.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
namespace Analysis;

public class StringifyMessages(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public void Stringify_Messages() // sometimes fails?
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

        Assembly assembly = typeof(InterReactClient).Assembly;

        List<Type> types = assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<MessageAttribute>() != null)
            .OrderBy(x => x.Name)
            .ToList();

        foreach (TypeInfo type in types)
        {
            try
            {
                //object instance = Stringifier.Instance.CreateInstance(type);
                //Assert.NotNull(instance);
                //string str = Stringifier.Instance.Stringify(instance);
                //Write($"Value: {str}");
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
