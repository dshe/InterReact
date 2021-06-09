namespace InterReact
{
    public sealed class NextId
    {
        public int Id { get; }
        internal NextId(ResponseReader c)
        {
            c.IgnoreVersion();
            Id = c.ReadInt();
        }
    }
}
