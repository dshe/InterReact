using System.Net;

namespace InterReact;

public static class IBDefaultPort
{
    public static readonly int Undefined;
    public static readonly int TwsRegularAccount = 7496;
    public static readonly int TwsDemoAccount = 7497;
    public static readonly int GatewayRegularAccount = 4001;
    public static readonly int GatewayDemoAccount = 4002;
}

public static partial class Extension
{
    internal static int[] IBDefaultPorts = [IBDefaultPort.TwsRegularAccount, IBDefaultPort.TwsDemoAccount, IBDefaultPort.GatewayRegularAccount, IBDefaultPort.GatewayDemoAccount];

    public static bool IsUsingIBDemoPort(this IPEndPoint endPoint)
    {
        ArgumentNullException.ThrowIfNull(endPoint);
        return endPoint.Port == IBDefaultPort.TwsDemoAccount
            || endPoint.Port == IBDefaultPort.GatewayDemoAccount;
    }
}
