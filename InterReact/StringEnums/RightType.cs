using StringEnums;

namespace InterReact.StringEnums
{
    /// <summary>
    /// Option Right Type
    /// </summary>
    public sealed class RightType : StringEnum<RightType>
    {
        /// <summary>
        /// Contract is not an option.
        /// </summary>
        public static readonly RightType Undefined = Create("0", "", "?");

        /// <summary>
        /// Option type is PUT (Right to sell).
        /// </summary>
        public static readonly RightType Put = Create("P");

        /// <summary>
        /// Option type is a CALL (Right to buy).
        /// </summary>
        public static readonly RightType Call = Create("C");
    }
}