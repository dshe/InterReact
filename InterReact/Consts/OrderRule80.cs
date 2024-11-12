namespace InterReact;

/// <summary>
/// Used for Rule 80A. Describes the type of trader.
/// </summary>
public static class OrderRule80
{
    public static readonly string None = "";
    public static readonly string Individual = "I";
    public static readonly string Agency = "A";
    public static readonly string AgentOtherMember = "W";
    public static readonly string IndividualPtia = "J";
    public static readonly string AgencyPtia = "U";
    public static readonly string AgentOtherMemberPtia = "M";
    public static readonly string IndividualPt = "K";
    public static readonly string AgencyPt = "Y";
    public static readonly string AgentOtherMemberPt = "N";
}
