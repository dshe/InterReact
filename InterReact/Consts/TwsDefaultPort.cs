using System.Net;
namespace InterReact;

public static class TwsDefaultPorts
{
    public static readonly int Undefined;
    public static readonly int RegularAccount = 7496;
    public static readonly int DemoAccount = 7497;
    public static readonly int GatewayRegularAccount = 4001;
    public static readonly int GatewayDemoAccount = 4002;
    internal static readonly int[] All = [RegularAccount, DemoAccount, GatewayRegularAccount, GatewayDemoAccount];
}

public static partial class Extensions
{
    extension(IPEndPoint endPoint)
    {
        public bool IsTwsDemoAccountPort()
        {
            ArgumentNullException.ThrowIfNull(endPoint);
            return endPoint.Port == TwsDefaultPorts.DemoAccount ||
                   endPoint.Port == TwsDefaultPorts.GatewayDemoAccount;
        }
    }
}
