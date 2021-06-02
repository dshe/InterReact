using System;

namespace InterReact.Utility
{
    internal sealed class BitMask
    {
        private readonly int Mask;
        internal BitMask(int i) => Mask = i;
        internal bool this[int index]
        {
            get
            {
                if (index < 0 || index >= 32)
                    throw new IndexOutOfRangeException();
                return (Mask & (1 << index)) != 0;
            }
        }
    }
}
