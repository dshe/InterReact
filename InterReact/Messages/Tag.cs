using InterReact.Core;
using InterReact.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class Tag // input + output
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        internal Tag() : this("","") { } // testing
        public Tag(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public Tag(ResponseComposer c) : this(c.ReadString(), c.ReadString()) { }

        internal static string Combine(IList<Tag>? tags)
        {
            if (tags == null || !tags.Any())
                return "";
            return tags.Select(tag => $"{tag.Name}={tag.Value}").JoinStrings(";");
        }
    }
}
