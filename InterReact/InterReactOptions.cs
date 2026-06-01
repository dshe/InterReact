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
    public IReadOnlyList<int> TwsPortAddresses { get; set; } = Extension.TwsDefaultPorts;
    /// <summary>
    /// Specify a client id. Up to 8 clients can attach to TWS/Gateway.
    /// Each client requires a unique Id. The default Id is random.
    /// </summary>
    public int TwsClientId { get; set; } = RandomNumberGenerator.GetInt32(100000, 1000000);
    public bool AllowOrderPlacement { get; set; }
    public string OptionalCapabilities { get; set; } = "";
    /// <summary>
    /// If UseDelayedTicks is true (default), delayed ticks are used for delayed market data.
    /// If UseDelayedTicks is false, non-delayed ticks are substituted for delayed ticks.
    /// </summary>
    public bool UseDelayedTicks { get; set; } = true;
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

    // ManagedAccounts is set in the ManagedAccounts message which is often received at startup.
    internal IReadOnlyList<string> ManagedAccounts { get; set; } = [];

    internal InterReactOptions(Action<InterReactOptions>? action)
    {
        action?.Invoke(this);
        if (LogFactory == NullLoggerFactory.Instance && Logger != NullLogger.Instance)
            LogFactory = Logger.ToLoggerFactory();
    }
}
