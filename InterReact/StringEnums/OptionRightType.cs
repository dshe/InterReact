using StringEnums;

namespace InterReact;

public sealed class OptionRightType : StringEnum<OptionRightType>
{
    /// <summary>
    /// Contract is not an option.
    /// </summary>
    public static OptionRightType Undefined { get; } = Create("0", "", "?");

    /// <summary>
    /// Option type is PUT.
    /// </summary>
    public static OptionRightType Put { get; } = Create("P");

    /// <summary>
    /// Option type is a CALL.
    /// </summary>
    public static OptionRightType Call { get; } = Create("C");
}
