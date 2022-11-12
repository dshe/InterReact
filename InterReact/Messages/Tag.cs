using System.Collections.Generic;
using System.Linq;
using Stringification;

namespace InterReact;

public sealed class Tag // input + output
{
    ///nint xx;
    public string Name { get; } = "";
    public string Value { get; } = "";
    internal Tag() { }
    public Tag(string name, string value)
    {
        Name = name;
        Value = value;
    }

    internal static string Combine(IEnumerable<Tag>? tags)
    {
        if (tags is null || !tags.Any())
            return "";

        return tags
            .Select(tag => $"{tag.Name}={tag.Value}")
            .JoinStrings(";");
    }
}
