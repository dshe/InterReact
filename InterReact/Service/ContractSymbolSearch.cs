using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;


namespace InterReact
{
    public sealed partial class Services
    {
        public async Task<IList<ContractDescription>> RequestMatchingSymbols(string pattern, CancellationToken ct = default)
        {
            var id = Request.GetNextId();
            var task = Response.OfType<IHasRequestId>().Where(x => x.RequestId == id).FirstAsync().ToTask(ct);
            Request.RequestMatchingSymbols(id, pattern);
            var result = await task.ConfigureAwait(false);
            if (result is Alert alert)
                throw alert;
            var symbolSamples = (SymbolSamples)result;
            return symbolSamples.Descriptions;
        }
    }
}
