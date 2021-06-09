using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using NodaTime;
using Stringification;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<List<ContractDescription>> CreateMatchingSymbolsObservable(string pattern) =>
            Response
                .ToObservableWithIdSingle<SymbolSamples>(Request.GetNextId, 
                    id => Request.RequestMatchingSymbols(id, pattern))
                .Select(m => m.Descriptions)
                .ToShareSource();
    }
}
