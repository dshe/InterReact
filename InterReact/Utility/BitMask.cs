namespace InterReact;

internal readonly struct BitMask
{
    private readonly int _mask;
    internal BitMask(int mask) => _mask = mask;
    internal bool this[int index]
    {
        get
        {
            if (index is < 0 or >= 32)
                throw new ArgumentException("Invalid", nameof(index));
            return (_mask & (1 << index)) != 0;
        }
    }
}
