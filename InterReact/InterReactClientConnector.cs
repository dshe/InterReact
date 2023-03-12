using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;

namespace InterReact;

public sealed record InterReactClientConnector
{
    internal IClock Clock { get; private init; } = SystemClock.Instance;
    internal InterReactClientConnector WithClock(IClock clock) => this with { Clock = clock };

    internal ILoggerFactory LogFactory { get; private init; } = NullLoggerFactory.Instance;
    public InterReactClientConnector WithLoggerFactory(ILoggerFactory loggerFactory) => this with
        { LogFactory = loggerFactory }; // preferable
    public InterReactClientConnector WithLogger(ILogger logger) => this with
        { LogFactory = logger.ToLoggerFactory() };

    internal IPAddress IpAddress { get; private init; } = IPAddress.Loopback;
    public InterReactClientConnector WithIpAddress(IPAddress address) => this with { IpAddress = address };

    internal int[] Ports { get; private init; } = Extension.AllIBPorts;
    /// <summary>
    /// Specify the port used to attempt connection to TWS/Gateway.
    /// If unspecified, connection will be attempted on ports 7496 and 7497, 4001, 4002.
    /// </summary>
    public InterReactClientConnector WithPort(int port) => this with { Ports = new[] { port } };

    internal int ClientId { get; private init; } = RandomNumberGenerator.GetInt32(100000, 1000000);
    /// <summary>
    /// Specify a client id.
    /// Up to 8 clients can attach to TWS/Gateway. Each client requires a unique Id. The default Id is random.
    /// </summary>
    public InterReactClientConnector WithClientId(int id) => this with 
        { ClientId = id >= 0 ? id : throw new ArgumentException("Invalid ClientId", id.ToString(CultureInfo.InvariantCulture))};

    internal int MaxRequestsPerSecond { get; private init; } = 50;
    /// <summary>
    /// Specify the maximum number of requests per second sent to to TWS/Gateway.
    /// </summary>
    public InterReactClientConnector WithMaxRequestsPerSecond(int rate) => this with
        { MaxRequestsPerSecond = rate > 0 ? rate : throw new ArgumentException("invalid", nameof(rate)) };

    internal string OptionalCapabilities { get; private init; } = "";
    public InterReactClientConnector WithOptionalCapabilities(string capabilities) => this with 
        { OptionalCapabilities = capabilities };

    // If tick is delayed, substitute with the corresponding n non-delayed tick. 
    internal bool UseDelayedTicks { get; private init; } = true; // default is true!
    public InterReactClientConnector DoNotUseDelayedTicks() => this with
        { UseDelayedTicks = false };

    /////////////////////////////////////////////////////////////

    public async Task<IInterReactClient> ConnectAsync(CancellationToken ct = default)
    {
        Connection connection = new(this);
        try
        {
            return await connection.ConnectAsync(ct).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            LogFactory.CreateLogger("InterReact.Connection").LogCritical(e, "ConnectAsync()");
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }
}
