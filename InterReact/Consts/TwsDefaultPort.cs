using System.Net;
namespace InterReact;

public static class TwsDefaultPorts
{
    public const int Undefined = 0;
    public const int RegularAccount = 7496;
    public const int DemoAccount = 7497;
    public const int GatewayRegularAccount = 4001;
    public const int GatewayDemoAccount = 4002;
    internal static readonly int[] All = [RegularAccount, DemoAccount, GatewayRegularAccount, GatewayDemoAccount];
}

public static partial class Xtensions
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
