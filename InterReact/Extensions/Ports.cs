using System.Net;

namespace InterReact;

public static partial class Extension
{
    public static readonly int[] AllIBPorts =
    {
        (int)IBDefaultPort.TwsRegularAccount,
        (int)IBDefaultPort.TwsDemoAccount,
        (int)IBDefaultPort.GatewayRegularAccount,
        (int)IBDefaultPort.GatewayDemoAccount
    };

    public static bool IsUsingIBDemoPort(this IPEndPoint endPoint)
    {
        ArgumentNullException.ThrowIfNull(endPoint);
        return endPoint.Port.IsIBDemoPort();
    }

    public static bool IsIBDemoPort(this int port) => port is 
        (int)IBDefaultPort.TwsDemoAccount or (int)IBDefaultPort.GatewayDemoAccount;
}
