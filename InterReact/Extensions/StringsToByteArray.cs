using System.Buffers.Binary;
using System.Text;
namespace InterReact;

public static partial class Xtensions
{
    extension(string str)
    {
        internal byte[] ToByteArray()
        {
            ArgumentNullException.ThrowIfNull(str);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            Array.Resize(ref bytes, bytes.Length + 1); // null terminator
            return bytes;
        }
    }

    extension(IEnumerable<string> strings)
    {
        internal byte[] ToByteArrayWithLengthPrefix() => strings.ToByteArray().AddLengthPrefix();
        private byte[] ToByteArray()
        {
            ArgumentNullException.ThrowIfNull(strings);
            return [..strings.SelectMany(s =>
            {
                ArgumentNullException.ThrowIfNull(s);
                return s.ToByteArray();
            })];
        }
    }

    extension(byte[] bytes)
    {
        private byte[] AddLengthPrefix()
        {
            ArgumentNullException.ThrowIfNull(bytes);
            byte[] buffer = new byte[bytes.Length + 4];
            BinaryPrimitives.WriteInt32BigEndian(buffer, bytes.Length);
            bytes.CopyTo(buffer, 4);
            return buffer;
        }
    }

}
