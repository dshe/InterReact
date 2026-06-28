namespace InterReact;

/// <summary>
/// Used for Rule 80A. Describes the type of trader.
/// </summary>
public sealed record OrderRule80(string Code) : IHasCode
{
    public static readonly OrderRule80 None  = new("");
    public static readonly OrderRule80 Individual  = new("I");
    public static readonly OrderRule80 Agency  = new("A");
    public static readonly OrderRule80 AgentOtherMember  = new("W");
    public static readonly OrderRule80 IndividualPtia  = new("J");
    public static readonly OrderRule80 AgencyPtia  = new("U");
    public static readonly OrderRule80 AgentOtherMemberPtia  = new("M");
    public static readonly OrderRule80 IndividualPt  = new("K");
    public static readonly OrderRule80 AgencyPt  = new("Y");
    public static readonly OrderRule80 AgentOtherMemberPt  = new("N");
}
