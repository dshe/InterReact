using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using InterReact.Messages;
using InterReact.Utility;

namespace InterReact.Extensions
{
    public static class ContractDataExpiryEx
    {
        /// <summary>
        /// Emits contract details with the n'th expiry.
        /// The default is 0, the next expiry.
        /// </summary>
        public static IObservable<IReadOnlyList<ContractData>> ContractDataExpirySelect(
            this IObservable<IReadOnlyList<ContractData>> source, int offset = 0)
        {
            if (offset < 0)
                throw new ArgumentException("Offset must be >= 0.");

            return ThrowIf.ThrowIfEmpty(source).Select(cds => ContractDataExpiryFilter(cds, offset));
        }

        internal static List<ContractData> ContractDataExpiryFilter(IEnumerable<ContractData> cds, int offset)
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
                return new List<ContractData>(); // empty

            return groups[offset].ToList();
        }
    }
}
