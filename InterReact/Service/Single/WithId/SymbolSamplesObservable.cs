using System;
using System.Reactive.Linq;
using System.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// Creates an observable which emits matching symbols for the pattern, then completes.
        /// Each subscription makes a separate request.
        /// </summary>
        public IObservable<Union<SymbolSamples, Alert>> CreateMatchingSymbolsObservable(string pattern) =>
            Response
                .ToObservableSingleWithId(
                    Request.GetNextId, id => Request.RequestMatchingSymbols(id, pattern))
                .Select(x => new Union<SymbolSamples, Alert>(x));
    }

}
