namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns a list of ContractDescriptions for the supplied pattern.
    /// TWS does not support more than one concurrent request.
    /// TWS error messages (AlertMessage) are directed to OnError(AlertException).
    /// </summary>
    public IObservable<IList<ContractDescription>> CreateMatchingSymbolsObservable(string pattern)
    {
        return Response
            .ToObservableSingleWithId(
                Request.GetNextId,
                id => Request.RequestMatchingSymbols(id, pattern))
            .AlertMessageToError()
            .Cast<SymbolSamples>()
            .Select(x => x.Descriptions)
            .FirstAsync()
            .ShareSource();
    }
}
