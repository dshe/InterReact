using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which emits AccountUpdateMulti messages.
    /// All messages are sent initially, and then only updates. 
    /// After sending all initial messages, a message with property "AccountUpdateMulti.IsEndMessage"=true is sent.
    /// The latest messages are cached for replay to new subscribers.
    /// </summary>
    public async Task<AccountSummary[]> GetAccountSummaryAsync()
    {
        int id = _request.GetNextId();

        // could be: data, end or error(throw)

        Task<AccountSummary[]> task = _response
            .WithRequestId(id)
            .Do(m =>
            {
                if (m is Alert a)
                    throw a.ToAlertException();
            })
            .OfTypeOnly<AccountSummary>()
            .TakeUntil(m => m.IsEndMessage)
            .ToArray()
            .ToTask();

        await _request.RequestAccountSummaryAsync(id).ConfigureAwait(false);

        AccountSummary[] s = await task.ConfigureAwait(false);

        await _request.CancelAccountSummaryAsync(id).ConfigureAwait(false);

        return s;
    }

    public async Task<AccountSummary[]> GetAccountSummary2Async()
    {
        int id = _request.GetNextId();

        Task<AccountSummary[]> task = _response
            .WithRequestId(id)
            .Select(m =>
            {
                if (m is Alert a)
                    throw a.ToAlertException();

                return m;
            })
            .TakeUntil(m => m is AccountSummaryEnd)
            .OfTypeOnly<AccountSummary>()
            .ToArray()
            .ToTask();

        await _request.RequestAccountSummaryAsync(id).ConfigureAwait(false);

        try
        {
            return await task.ConfigureAwait(false);
        }
        finally
        {
            await _request.CancelAccountSummaryAsync(id).ConfigureAwait(false);
        }
    }
}
