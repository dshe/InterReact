using InterReact.Core;

namespace InterReact.Messages
{
    public sealed class Tag // input + output
    {
        public string Name { get; }
        public string Value { get; }
        public Tag(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public Tag(ResponseReader c) : this(c.ReadString(), c.ReadString()) { }
    }
}
