using Stringification;

namespace InterReact;

public sealed class Tag // input + output
{
    public string Name { get; }
    public string Value { get; }

    public Tag(string name, string value) => (Name, Value) = (name, value);

    internal static string Combine(IEnumerable<Tag>? tags)
    {
        if (tags is null)
            return "";

        List<Tag> tagsList = tags.ToList();
        if (tagsList.Count == 0)
            return "";

        return tagsList
            .Select(tag => $"{tag.Name}={tag.Value}")
            .JoinStrings(";");
    }
}
