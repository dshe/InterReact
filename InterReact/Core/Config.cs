using System.Net;
using System;
using NodaTime;

namespace InterReact
{
    public sealed class Config : EditorBrowsableNever
    {
        private static readonly Random Random = new();

        internal Config() { }
        internal IClock Clock { get; set; } = SystemClock.Instance;

        public IPEndPoint IPEndPoint { get; internal set; } = new(IPAddress.IPv6Loopback, 0);
        public int[] Ports { get; internal set; } = { (int)DefaultPort.TwsRegularAccount, (int)DefaultPort.TwsDemoAccount, (int)DefaultPort.GatewayRegularAccount, (int)DefaultPort.GatewayDemoAccount };
        public bool IsDemoAccount => IPEndPoint.Port == (int)DefaultPort.TwsDemoAccount || IPEndPoint.Port == (int)DefaultPort.GatewayDemoAccount;
        public int ClientId { get; internal set; } = Random.Next(1000, 1000000);
        public int MaxRequestsPerSecond { get; internal set; } = 50;
        public string OptionalCapabilities { get; internal set; } = "";

        /// <summary>
        /// The version of the currently connected server.
        /// </summary>
        public ServerVersion ServerVersionCurrent { get; internal set; }
        public const ServerVersion ServerVersionMin = ServerVersion.VT100; // always 100
        public const ServerVersion ServerVersionMax = ServerVersion.UnderlyingInfo;

        public string ManagedAccounts { get; internal set; } = "";
        public string Date { get; internal set; } = "";
        internal int NextIdValue;

        internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;
        internal void RequireServerVersion(ServerVersion version)
        {
            if (!SupportsServerVersion(version))
                throw new ArgumentException($"The server does not support version: '{version}'.");
        }
    }
}
