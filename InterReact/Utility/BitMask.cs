using System;

namespace InterReact.Utility
{
    internal sealed class BitMask
    {
        internal int Mask { get; private set; }

        internal BitMask(int i) => Mask = i;

        internal void Clear() => Mask = 0;

        internal bool this[int index]
        {
            get
            {
                if (index < 0 || index >= 32)
                    throw new IndexOutOfRangeException();
                return (Mask & (1 << index)) != 0;
            }
            set
            {
                if (index < 0 || index >= 32)
                    throw new IndexOutOfRangeException();
                if (value)
                    Mask |= 1 << index;
                else
                    Mask &= ~(1 << index);
            }
        }
    }
}
