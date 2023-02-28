using Stringification;

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
        if (tags is null)
            return "";

        List<Tag> tagsList = tags.ToList();
        if (!tagsList.Any())
            return "";

        return tagsList
            .Select(tag => $"{tag.Name}={tag.Value}")
            .JoinStrings(";");
    }
}
