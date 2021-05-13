using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    public static class ContractDataExpiryExtensions
    {
        /// <summary>
        /// Emits contract details with the n'th expiry.
        /// The default is 0, the next expiry.
        /// </summary>
        public static IObservable<IReadOnlyList<ContractDetails>> ContractDataExpirySelect(
            this IObservable<IReadOnlyList<ContractDetails>> source, int offset)
        {
            if (offset < 0)
                throw new ArgumentException("Offset must be >= 0.");

            return source.ThrowIfAnyEmpty().Select(cds => ContractDataExpiryFilter(cds, offset));
        }

        public static List<ContractDetails> ContractDataExpiryFilter(this IEnumerable<ContractDetails> cds, int offset)
        {
            var groups = cds
                .GroupBy(cd => cd.Contract.LastTradeDateOrContractMonth)
                .OrderBy(g => g.Key)
                .ToList();

            if (groups.Any(g => string.IsNullOrEmpty(g.Key)))
                throw new InvalidDataException("Empty expiry found.");

            if (offset == int.MaxValue)
                return groups.Last().ToList();

            if (offset >= groups.Count) // invalid expiry offset
                return new List<ContractDetails>(); // empty

            return groups[offset].ToList();
        }
    }
}
