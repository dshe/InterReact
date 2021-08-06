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
        public int ClientId { get; internal set; } = Random.Next(1000, 1000000);
        public int MaxRequestsPerSecond { get; set; } = 50;
        public string OptionalCapabilities { get; internal set; } = "";

        /// <summary>
        /// The version of the currently connected server.
        /// </summary>
        public int ServerVersionCurrent { get; internal set; }
        public const int ServerVersionMin = ServerVersion.FRACTIONAL_POSITIONS;
        public const int ServerVersionMax = ServerVersion.DURATION;

        public string Date { get; internal set; } = "";

        internal bool SupportsServerVersion(int version) => version <= ServerVersionCurrent;
        internal void RequireServerVersion(int version)
        {
            if (!SupportsServerVersion(version))
                throw new ArgumentException($"The server does not support version: '{version}'.");
        }
    }
}
