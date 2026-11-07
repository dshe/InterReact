using System.Globalization;
using System.IO;
using System.Net;
namespace InterReact;

public sealed class Connection(SocketWrapper socketWrapper, InterReactOptions options) : IAsyncDisposable
{
    private readonly TimeSpan _loginReceiveTimeout = TimeSpan.FromSeconds(3); // Timeout.InfiniteTimeSpan;
    private readonly InterReactOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger _logger = options.LogFactory.CreateLogger<Connection>();
    private readonly CancellationTokenSource _cts = new();
    internal CancellationToken Ct => _cts.Token;
    internal IPEndPoint RemoteIpEndPoint => socketWrapper.RemoteIpEndPoint;
    internal IObservable<string[]> ConnectionObservable { get; private set; } = Observable.Empty<string[]>();
    private int _disposed;

    internal async Task StartAsync(CancellationToken ct)
    {
        await LoginAsync(ct).ConfigureAwait(false);
        ConnectionObservable = CreateObservable();
    }

    private async Task LoginAsync(CancellationToken ct)
    {
        await socketWrapper.WriteStringAsync("API", ct).ConfigureAwait(false); // Send without prefix

        // Send with prefix. Report a range of supported API versions to TWS.
        string s = $"v{(int)_options.ServerVersionMin}..{(int)_options.ServerVersionMax}";
        await socketWrapper.WriteMessageAsync([s], ct).ConfigureAwait(false);

        await socketWrapper.WriteMessageAsync([
                ((int)RequestCode.StartApi).ToString(CultureInfo.InvariantCulture),
                "2",
                _options.TwsClientId.ToString(CultureInfo.InvariantCulture),
                _options.OptionalCapabilities
            ], ct).ConfigureAwait(false);

        string[] firstMessage = await socketWrapper.ReceiveOneMessageWithTimoutAsync(_loginReceiveTimeout, ct).ConfigureAwait(false);
        // ServerVersion is the highest supported API version within the range specified.
        if (!Enum.TryParse(firstMessage[0], out ServerVersion version))
            throw new InvalidDataException($"Could not parse server version '{firstMessage[0]}'.");
        _options.ServerVersionCurrent = version;
        if (_options.ServerVersionCurrent < _options.ServerVersionMin || _options.ServerVersionCurrent > _options.ServerVersionMax)
            throw new InvalidDataException($"Invalid server version '{_options.ServerVersionCurrent}'.");
        // The rest of the message is ignored and contains the date, time and timezone.

        _logger.LogInformation("Logged into TWS/Gateway version {ServerVersionCurrent} with ClientId {ClientId}.",
            (int)_options.ServerVersionCurrent, _options.TwsClientId);
    }

    internal IObservable<string[]> CreateObservable()
    {
        return Observable.Create<string[]>(async observer =>
        {
            _logger.LogInformation("ConnectionObservable Observing.");
            try
            {
                while (!Ct.IsCancellationRequested)
                {
                    string[] message = await socketWrapper.ReceiveOneMessageAsync(Ct).ConfigureAwait(false);
                    observer.OnNext(message);
                }
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                if (Ct.IsCancellationRequested)
                    observer.OnCompleted();
                else
                    observer.OnError(ex);
            }
        });
    }

    internal async ValueTask SendMessageAsync(IEnumerable<string> strings) =>
        await socketWrapper.WriteMessageAsync(strings, Ct).ConfigureAwait(false);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        await _cts.CancelAsync().ConfigureAwait(false);
        await socketWrapper.DisposeAsync().ConfigureAwait(false);

        _cts.Dispose();
        _logger.LogInformation("Disposed.");
    }
}
