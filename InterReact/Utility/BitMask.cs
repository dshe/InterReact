using System;

namespace InterReact.Utility
{
    internal sealed class BitMask
    {
        public int Mask { get; private set; }

        public BitMask(int i) => Mask = i;

        public void Clear() => Mask = 0;

        public bool this[int index]
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
