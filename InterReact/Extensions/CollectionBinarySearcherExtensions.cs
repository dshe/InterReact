using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InterReact.Extensions
{
    internal static class CollectionBinarySearcherExtensions
    {
        internal static int BinarySearch<T>(this Collection<T> collection, T value, IComparer<T> comparer)
        {
            if (collection.Count == 0)
                return -1;

            int lo = 0;
            int hi = collection.Count - 1;

            while (lo <= hi)
            {
                int i = getMedian(lo, hi);
                int c;
                try
                {
                    c = comparer.Compare(collection[i], value);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("IComparer failure.", e);
                }
                if (c == 0)
                    return i;
                if (c < 0)
                    lo = i + 1;
                else
                    hi = i - 1;
            }
            return ~lo;

            // local
            int getMedian(int low, int high) => low + ((high - low) >> 1);
        }
    }
}
