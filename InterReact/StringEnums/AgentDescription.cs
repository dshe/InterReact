using StringEnums;

namespace InterReact;

/// <summary>
/// Used for Rule 80A. Describes the type of trader.
/// </summary>
public sealed class AgentDescription : StringEnum<AgentDescription>
{
    public static AgentDescription None { get; } = Create("");
    public static AgentDescription Individual { get; } = Create("I");
    public static AgentDescription Agency { get; } = Create("A");
    public static AgentDescription AgentOtherMember { get; } = Create("W");
    public static AgentDescription IndividualPtia { get; } = Create("J");
    public static AgentDescription AgencyPtia { get; } = Create("U");
    public static AgentDescription AgentOtherMemberPtia { get; } = Create("M");
    public static AgentDescription IndividualPt { get; } = Create("K");
    public static AgentDescription AgencyPt { get; } = Create("Y");
    public static AgentDescription AgentOtherMemberPt { get; } = Create("N");
}
