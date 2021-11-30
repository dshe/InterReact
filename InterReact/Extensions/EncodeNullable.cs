namespace InterReact;

public static partial class Extensions
{
    internal static int EncodeNullable(this int? i) => i ?? int.MaxValue;
    internal static int? DecodeNullable(this int i) => i == int.MaxValue ? null : i;

    internal static double EncodeNullable(this double? d) => d ?? double.MaxValue;
    internal static double? DecodeNullable(this double d) => d == double.MaxValue ? null : d;
}
