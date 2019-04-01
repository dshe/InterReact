using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using InterReact.Messages;
using InterReact.Utility;

namespace InterReact.Extensions
{
    public static class ContractDataSmartEx
    {
        /// <summary>
        /// For each particular contractId, if SMART is one of the exchanges, this filter removes the contracts with other exchanges.
        /// </summary>
        public static IObservable<IReadOnlyList<ContractData>> ContractDataSmartFilter(this IObservable<IReadOnlyList<ContractData>> source)
            => ThrowIf.ThrowIfEmpty(source).Select(ContractDataSmartFilter);

        internal static List<ContractData> ContractDataSmartFilter(IEnumerable<ContractData> cds)
        {
            var list = new List<ContractData>();
            foreach (var group in cds.GroupBy(cd => cd.Contract.ContractId))
            {
                var smart = group.Where(cd => cd.Contract.Exchange == "SMART").ToList();
                if (smart.Any())
                    list.AddRange(smart);
                else
                    list.AddRange(group);
            }
            return list;
        }
    }

}
