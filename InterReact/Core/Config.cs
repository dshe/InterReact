using System.Net;
using InterReact.Interfaces;
using System;
using InterReact.Enums;
using NodaTime;

namespace InterReact.Core
{
    public sealed class Config : EditorBrowsableNever
    {
        internal static Random Random = new Random();

        public IClock Clock { get; internal set; } = SystemClock.Instance;

        public DateTimeZone SystemTimeZone { get; internal set; } = DateTimeZoneProviders.Tzdb.GetSystemDefault();

        public IPEndPoint IPEndPoint { get; internal set; } = new IPEndPoint(IPAddress.IPv6Loopback, 0);

        internal int[] Ports = { 4001, 4002, 7496, 7497 };

        public bool IsDemoAccount => IPEndPoint.Port == 4002 || IPEndPoint.Port == 7497;

        public int ClientId { get; internal set; } = Random.Next(1000, 1000000);

        public int    MaxRequestsPerSecond { get; internal set; } = 50;
        public string OptionalCapabilities { get; internal set; }

        /// <summary>
        /// The maximum version of the API supported by this client.
        /// </summary>
        public const string ClientApiVersion = "9.73.05";

        /// <summary>
        /// The version of the currently connected server.
        /// </summary>
        public ServerVersion ServerVersionCurrent { get; internal set; }

        public const ServerVersion ServerVersionMin = ServerVersion.UseV100Plus;
        public const ServerVersion ServerVersionMax = ServerVersion.AggGroup;

        public string ManagedAccounts { get; internal set; }
        public string Date            { get; internal set; }

        internal int  NextIdValue; // set during login

        internal bool SupportsServerVersion(ServerVersion version) => version <= ServerVersionCurrent;

        internal void RequireServerVersion(ServerVersion version)
        {
            if (!SupportsServerVersion(version))
                throw new ArgumentException($"The server does not support version: '{version}'.");
        }

        internal Config() {}
    }

}
