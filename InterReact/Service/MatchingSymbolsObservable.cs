using System;
using System.Reactive.Linq;
using System.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<ISymbolSamples> CreateMatchingSymbolsObservable(string pattern) =>
            Response
                .ToObservableWithIdSingle<ISymbolSamples>(Request.GetNextId, 
                    id => Request.RequestMatchingSymbols(id, pattern))
                .ToShareSource();
    }

    public static partial class Extensions
    {
        public static IObservable<SymbolSamples> OfTypeSymbolSamples(this IObservable<ISymbolSamples> source) =>
            source.OfType<SymbolSamples>();
    }

}
