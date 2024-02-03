using Stringification;
using System.Globalization;
using System.Net;

namespace InterReact;

public static partial class Extension
{
    internal static string GetDefaultPorts()
    {   // allows order to be specified
        return new IBDefaultPort[]
        {
            IBDefaultPort.TwsRegularAccount,
            IBDefaultPort.TwsDemoAccount,
            IBDefaultPort.GatewayRegularAccount,
            IBDefaultPort.GatewayDemoAccount
        }
        .Select(e => (int)e)
        .Select(n => n.ToString(CultureInfo.InvariantCulture))
        .JoinStrings(", ");
    }

    public static bool IsUsingIBDemoPort(this IPEndPoint endPoint)
    {
        ArgumentNullException.ThrowIfNull(endPoint);
        return endPoint.Port.IsIBDemoPort();
    }

    public static bool IsIBDemoPort(this int port) => port is
        (int)IBDefaultPort.TwsDemoAccount or (int)IBDefaultPort.GatewayDemoAccount;
}
