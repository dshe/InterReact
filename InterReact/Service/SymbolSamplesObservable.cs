using System;
using System.Reactive.Linq;
using System.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<Union<SymbolSamples, Alert>> CreateMatchingSymbolsObservable(string pattern) =>
            Response
                .ToObservableWithIdSingle(Request.GetNextId, 
                    id => Request.RequestMatchingSymbols(id, pattern))
                .Select(x => new Union<SymbolSamples, Alert>(x))
                .ToShareSource();
    }

}
