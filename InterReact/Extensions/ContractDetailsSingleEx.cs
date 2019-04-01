using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using InterReact.Messages;
using StringEnums;
using InterReact.Extensions;
using InterReact.Utility;

namespace InterReact.Extensions
{
    public static class ContractDataSingleEx
    {
        /// <summary>
        /// Completes successfully if the sequence contains exactly one contract details object.
        /// Otherwise, OnError is called containing an exception with a message indicating which properties are different. 
        /// </summary>
        public static IObservable<ContractData> ContractDataSingle(this IObservable<IReadOnlyList<ContractData>> source)
        {
            
            return ThrowIf.ThrowIfEmpty(source).Select(cds =>
            {
                if (cds.Count == 1)
                    return cds.Single();
                throw new InvalidDataException(ErrorMessage(cds));
            });
        }

        private static string ErrorMessage(IReadOnlyList<ContractData> cds)
        {
            var c = new Contract();
            var sb = new StringBuilder();

            foreach (var property in typeof(Contract).GetTypeInfo()
                .DeclaredProperties
                .Where(p => p.Name != nameof(c.ContractId) && p.Name != nameof(c.LocalSymbol))
                .OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                var values = cds
                    .Select(cd => property.GetValue(cd.Contract))
                    .Select(value => value?.ToString() ?? "(null)") // should not be null
                    .Distinct()
                    .OrderBy(value => value, StringComparer.Ordinal);

                if (values.Count() > 1)
                    sb.AppendFormat($"{property.Name}: {values.JoinStrings(", ")}"+ Environment.NewLine+Environment.NewLine);
            }

            if (sb.Length == 0)
                return "Unexpected: no duplicate properties found.";

            return $"{cds.Count} contracts found which differ by:" + Environment.NewLine + Environment.NewLine + sb;
        }
    }
}
