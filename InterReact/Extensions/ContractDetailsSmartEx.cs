using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    public static class ContractDataSmartExtensions
    {
        /// <summary>
        /// For each particular contractId, if SMART is one of the exchanges, this filter removes the contracts with other exchanges.
        /// </summary>
        public static IObservable<IReadOnlyList<ContractDetails>> ContractDataSmartFilter(this IObservable<IReadOnlyList<ContractDetails>> source)
            => source.ThrowIfAnyEmpty().Select(ContractDataSmartFilter);

        internal static List<ContractDetails> ContractDataSmartFilter(IEnumerable<ContractDetails> cds)
        {
            var list = new List<ContractDetails>();
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
