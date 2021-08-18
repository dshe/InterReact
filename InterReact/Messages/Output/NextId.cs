namespace InterReact
{
    public sealed class NextId
    {
        public int Id { get; }
        internal NextId(ResponseReader r)
        {
            r.IgnoreVersion();
            Id = r.ReadInt();
        }
    }
}
