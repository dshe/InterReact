using System.Net;
using NodaTime;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InterReact;

public class Config
{
    internal IClock Clock { get; set; } = SystemClock.Instance;
    internal ILogger Logger { get; set; } = NullLogger.Instance;
    internal bool LogIncomingMessages { get; set; }

    public IPEndPoint IPEndPoint { get; internal set; } = new(IPAddress.IPv6Loopback, 0);
    public IReadOnlyList<int> Ports { get; internal set; } = new[] { (int)DefaultPort.TwsRegularAccount, (int)DefaultPort.TwsDemoAccount, (int)DefaultPort.GatewayRegularAccount, (int)DefaultPort.GatewayDemoAccount };
    public bool IsDemoAccount => IPEndPoint.Port == (int)DefaultPort.TwsDemoAccount || IPEndPoint.Port == (int)DefaultPort.GatewayDemoAccount;
    public int ClientId { get; internal set; } = RandomNumberGenerator.GetInt32(100000, 999999);
    public int MaxRequestsPerSecond { get; internal set; } = 50;
    public string OptionalCapabilities { get; internal set; } = "";
    public bool FollowPriceTickWithSizeTick { get; internal set; }

    public ServerVersion ServerVersionCurrent { get; internal set; } = ServerVersion.NONE;
    public ServerVersion ServerVersionMin { get; } = ServerVersion.FRACTIONAL_POSITIONS;
    public ServerVersion ServerVersionMax { get; internal set; } = ServerVersion.WSHE_CALENDAR;

    public string Date { get; internal set; } = "";

    internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;
    internal void RequireServerVersion(ServerVersion version)
    {
        if (!SupportsServerVersion(version))
            throw new ArgumentException($"The server does not support version: '{version}'.");
    }
}
