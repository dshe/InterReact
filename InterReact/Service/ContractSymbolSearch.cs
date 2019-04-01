using System;
using System.Reactive.Linq;
using InterReact.Messages;
using InterReact.Utility.Rx;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// This function is still in development.
        /// </summary>
        public IObservable<SymbolSamples> ContractSymbolSearch(string pattern) =>
            Response
                .ToObservable<SymbolSamples>(
                    Request.NextId,
                    requestId => Request.RequestMatchingSymbols(requestId, pattern))
                .ToShareSource();
    }
}
