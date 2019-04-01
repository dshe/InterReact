using InterReact.Core;

namespace InterReact.Messages
{
    public sealed class NextId
    {
        public int Id { get; }
        internal NextId(ResponseComposer c)
        {
            c.IgnoreVersion();
            Id = c.ReadInt();
        }
    }
}
