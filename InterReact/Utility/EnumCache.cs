using System.Globalization;
namespace InterReact;

// Generic Static Cache !

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000: Do not declare static members on generic types", Justification = "Intentional per-closed-generic cache")]
public static class EnumCache<T> where T : struct, Enum
{
    public static readonly HashSet<long> Values =
        [.. Enum.GetValues<T>().Select(v =>
            Convert.ToInt64(v, CultureInfo.InvariantCulture))];

    public static bool IsDefined(long value)
        => Values.Contains(value);
}
