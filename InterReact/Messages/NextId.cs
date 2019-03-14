using InterReact.Core;

namespace InterReact.Messages
{
    public sealed class NextId
    {
        public int Id { get; }
        internal NextId(ResponseReader c)
        {
            c.IgnoreVersion();
            Id = c.Read<int>();
        }
    }
}
