using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// For each particular contractId, if SMART is one of the exchanges, this filter removes the contracts with other exchanges.
        /// </summary>
        public static IObservable<IReadOnlyList<ContractDetails>> ContractDataSmartFilter(this IObservable<IReadOnlyList<ContractDetails>> source)
            => source.ThrowIfAnyEmpty().Select(ContractDataSmartFilter);

        internal static List<ContractDetails> ContractDataSmartFilter(IEnumerable<ContractDetails> cds)
        {
            List<ContractDetails> list = new();
            foreach (var group in cds.GroupBy(cd => cd.Contract.ContractId))
            {
                List<ContractDetails> smart = group.Where(cd => cd.Contract.Exchange == "SMART").ToList();
                if (smart.Any())
                    list.AddRange(smart);
                else
                    list.AddRange(group);
            }
            return list;
        }
    }

}
