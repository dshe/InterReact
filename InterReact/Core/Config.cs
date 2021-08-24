using System.Net;
using System;
using NodaTime;

namespace InterReact
{
    public class Config : EditorBrowsableNever
    {
        private static readonly Random Random = new();

        internal Config() { }
        internal IClock Clock { get; set; } = SystemClock.Instance;
        internal bool LogIncomingMessages { get; set; }

        public IPEndPoint IPEndPoint { get; internal set; } = new(IPAddress.IPv6Loopback, 0);
        public int[] Ports { get; internal set; } = { (int)DefaultPort.TwsRegularAccount, (int)DefaultPort.TwsDemoAccount, (int)DefaultPort.GatewayRegularAccount, (int)DefaultPort.GatewayDemoAccount };
        public bool IsDemoAccount => IPEndPoint.Port == (int)DefaultPort.TwsDemoAccount || IPEndPoint.Port == (int)DefaultPort.GatewayDemoAccount;
        public int ClientId { get; internal set; } = Random.Next(100000, 999999);
        public int MaxRequestsPerSecond { get; internal set; } = 50;
        public string OptionalCapabilities { get; internal set; } = "";

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
}
