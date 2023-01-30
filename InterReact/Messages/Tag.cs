using Stringification;
using System.Collections.Generic;
using System.Linq;

namespace InterReact;

public sealed class Tag // input + output
{
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public Tag() { }
    internal Tag(ResponseReader r)
    {
        Name = r.ReadString();
        Value = r.ReadString();
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
