using System.Net;

namespace InterReact;

public static class DefaultPort
{
    public const int Undefined = 0;
    public const int TwsRegularAccount = 7496;
    public const int TwsDemoAccount = 7497;
    public const int GatewayRegularAccount = 4001;
    public const int GatewayDemoAccount = 4002;
}

public static partial class Extension
{
    internal static int[] IBDefaultPorts = [DefaultPort.TwsRegularAccount, DefaultPort.TwsDemoAccount, DefaultPort.GatewayRegularAccount, DefaultPort.GatewayDemoAccount];

    public static bool IsUsingIBDemoPort(this IPEndPoint endPoint)
    {
        ArgumentNullException.ThrowIfNull(endPoint);
        return endPoint.Port is
            DefaultPort.TwsDemoAccount or DefaultPort.GatewayDemoAccount;
    }
}
