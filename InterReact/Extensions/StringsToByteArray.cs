using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
namespace InterReact;

internal static partial class Extensions
{
    internal static byte[] ToByteArrayWithLengthPrefix(this IEnumerable<string> strings) =>
        strings.ToByteArray().ToByteArrayWithLengthPrefix();

    private static byte[] ToByteArray(this IEnumerable<string> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return [..source.SelectMany(s =>
        {
            ArgumentNullException.ThrowIfNull(s);
            return s.ToByteArray();
        })];
    }

    internal static byte[] ToByteArray(this string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        byte[] bytes = Encoding.UTF8.GetBytes(source);
        Array.Resize(ref bytes, bytes.Length + 1); // null terminator
        return bytes;
    }

    private static byte[] ToByteArrayWithLengthPrefix(this byte[] source)
    {
        ArgumentNullException.ThrowIfNull(source);
        byte[] buffer = new byte[source.Length + 4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, source.Length);
        source.CopyTo(buffer, 4);
        return buffer;
    }
}
