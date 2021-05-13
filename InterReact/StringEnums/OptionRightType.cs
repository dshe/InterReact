using StringEnums;

namespace InterReact
{
    public sealed class OptionRightType : StringEnum<OptionRightType>
    {
        /// <summary>
        /// Contract is not an option.
        /// </summary>
        public static readonly OptionRightType Undefined = Create("0", "", "?");

        /// <summary>
        /// Option type is PUT.
        /// </summary>
        public static readonly OptionRightType Put = Create("P");

        /// <summary>
        /// Option type is a CALL.
        /// </summary>
        public static readonly OptionRightType Call = Create("C");
    }
}