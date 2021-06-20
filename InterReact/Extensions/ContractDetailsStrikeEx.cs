using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// Selects contract(s) from a list of contracts with varying expiry dates.
        /// The reference price, in combination with the offset, is used to select contract(s) depending on strike.
        /// An offset of 0 returns contract(s) with a strike which is equal of greater to the reference price.
        /// An offset of +1 returns contract(s) with the next greater strike.
        /// Negative Values select contract(s) with strikes below the reference price.
        /// </summary>
        public static IObservable<IReadOnlyList<ContractDetails>> ContractDataStrikeSelect(
                this IObservable<IReadOnlyList<ContractDetails>> source, int offset, double referencePrice)
            => source.ThrowIfAnyEmpty().Select(cds => SelectStrike(cds, offset, referencePrice));

        internal static List<ContractDetails> SelectStrike(IEnumerable<ContractDetails> cds, int offset, double reference)
        {
            var groups = cds
                .GroupBy(cd => cd.Contract.Strike)
                .OrderBy(g => g.Key)
                .ToList();

            List<double> strikes = groups.Select(y => y.Key).ToList();

            if (strikes.Any(key => key <= 0))
                throw new InvalidDataException("Invalid strike.");

            int pos = strikes.BinarySearch(reference);
            if (pos < 0)
                pos = ~pos;

            int index = offset + pos;

            if (index < 0 || index > groups.Count - 1) // invalid index
                return new List<ContractDetails>();

            return groups[index].ToList();
        }

    }
}
