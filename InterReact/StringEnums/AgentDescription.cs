using StringEnums;

namespace InterReact
{
    /// <summary>
    /// Used for Rule 80A. Describes the type of trader.
    /// </summary>
    public sealed class AgentDescription : StringEnum<AgentDescription>
    {
        public static readonly AgentDescription None = Create("");
        public static readonly AgentDescription Individual = Create("I");
        public static readonly AgentDescription Agency = Create("A");
        public static readonly AgentDescription AgentOtherMember = Create("W");
        public static readonly AgentDescription IndividualPtia = Create("J");
        public static readonly AgentDescription AgencyPtia = Create("U");
        public static readonly AgentDescription AgentOtherMemberPtia = Create("M");
        public static readonly AgentDescription IndividualPt = Create("K");
        public static readonly AgentDescription AgencyPt = Create("Y");
        public static readonly AgentDescription AgentOtherMemberPt = Create("N");
    }

}