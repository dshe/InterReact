using System.Collections.Generic;
using System.Linq;
using Stringification;

namespace InterReact
{
    public sealed class Tag // input + output
    {
        public string Name { get; } = "";
        public string Value { get; } = "";
        internal Tag() { }
        public Tag(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public Tag(ResponseReader r) : this(r.ReadString(), r.ReadString())  {}

        internal static string Combine(IEnumerable<Tag>? tags)
        {
            if (tags == null || !tags.Any())
                return "";

            return tags
                .Select(tag => $"{tag.Name}={tag.Value}")
                .JoinStrings(";");
        }
    }
}
