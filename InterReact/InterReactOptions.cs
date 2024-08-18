using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Security.Cryptography;

namespace InterReact;

public sealed class InterReactOptions
{
    internal IClock Clock { get; set; } = SystemClock.Instance; // for testing
    public ILogger Logger { get; set; } = NullLogger.Instance;
    public ILoggerFactory LogFactory { get; set; } = NullLoggerFactory.Instance;
    /// <summary>
    /// Specify the IP address of TWS/Gateway if not the local host.
    /// </summary>
    public IPAddress TwsIpAddress { get; set; } = IPAddress.IPv6Loopback;
    /// <summary>
    /// Specify the port(s) used to attempt connection to TWS/Gateway.
    /// If unspecified, connection will be attempted on ports 7496 and 7497, 4001, 4002.
    /// </summary>
    public IEnumerable<int> IBPortAddresses { get; set; } = Extension.IBDefaultPorts;
    /// <summary>
    /// Specify a client id. Up to 8 clients can attach to TWS/Gateway.
    /// Each client requires a unique Id. The default Id is random.
    /// </summary>
    public int TwsClientId { get; set; } = RandomNumberGenerator.GetInt32(100000, 1000000);

    public int MaxRequestsPerSecond { get; set; } = 50;
    public string OptionalCapabilities { get; set; } = "";
    /// <summary>
    /// If tick is delayed, substitute with the corresponding non-delayed tick. 
    /// </summary>
    public bool UseDelayedTicks { get; set; } = true; // default is true!

    public ServerVersion ServerVersionMin { get; } = ServerVersion.BOND_ISSUERID;
    public ServerVersion ServerVersionMax  { get; } = ServerVersion.BOND_ISSUERID;
    public ServerVersion ServerVersionCurrent { get; internal set; } = ServerVersion.NONE;
    internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;
    internal void RequireServerVersion(ServerVersion version)
    {
        if (!SupportsServerVersion(version))
            throw new ArgumentException($"The server does not support version: '{version}'.");
    }

    // Id is incremented by method Request.GetNextId().
    // Id is updated in the constructor of the NextOrderId message which is received at startup.
    // The NextOrderId message is also received in response to Request.RequestNextOrderId().
    internal int Id;

    internal InterReactOptions(Action<InterReactOptions>? action)
    {
        action?.Invoke(this);
        if (LogFactory == NullLoggerFactory.Instance && Logger != NullLogger.Instance)
            LogFactory = Logger.ToLoggerFactory();
        if (!IBPortAddresses.Any())
            throw new InvalidOperationException("InterReactOptions: missing port address.");
        if (MaxRequestsPerSecond <= 0)
            throw new InvalidOperationException("InterReactOptions: invalid MaxRequestsPerSecond.");
    }
}
