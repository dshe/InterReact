using System.Net;
namespace InterReact;

public static class TwsDefaultPort
{
    public static readonly int Undefined;
    public static readonly int RegularAccount = 7496;
    public static readonly int DemoAccount = 7497;
    public static readonly int GatewayRegularAccount = 4001;
    public static readonly int GatewayDemoAccount = 4002;
}

public static partial class Extension
{
    internal static int[] TwsDefaultPorts = [TwsDefaultPort.RegularAccount, TwsDefaultPort.DemoAccount, TwsDefaultPort.GatewayRegularAccount, TwsDefaultPort.GatewayDemoAccount];

    public static bool IsUsingTwsDemoPort(this IPEndPoint endPoint)
    {
        ArgumentNullException.ThrowIfNull(endPoint);
        return endPoint.Port == TwsDefaultPort.DemoAccount
            || endPoint.Port == TwsDefaultPort.GatewayDemoAccount;
    }
}
