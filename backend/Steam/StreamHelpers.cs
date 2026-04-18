using System.Globalization;
using System.Text;

namespace SAM.Backend.Steam;

internal static class StreamHelpers
{
    public static byte ReadValueU8(this Stream stream)
    {
        int b = stream.ReadByte();
        if (b < 0)
        {
            throw new InvalidDataException("Unexpected end of stream");
        }
        return (byte)b;
    }

    public static int ReadValueS32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[4];
        if (stream.Read(data) != 4)
        {
            throw new InvalidDataException("Failed to read 4 bytes");
        }
        return BitConverter.ToInt32(data);
    }

    public static uint ReadValueU32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[4];
        if (stream.Read(data) != 4)
        {
            throw new InvalidDataException("Failed to read 4 bytes");
        }
        return BitConverter.ToUInt32(data);
    }

    public static ulong ReadValueU64(this Stream stream)
    {
        Span<byte> data = stackalloc byte[8];
        if (stream.Read(data) != 8)
        {
            throw new InvalidDataException("Failed to read 8 bytes");
        }
        return BitConverter.ToUInt64(data);
    }

    public static float ReadValueF32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[4];
        if (stream.Read(data) != 4)
        {
            throw new InvalidDataException("Failed to read 4 bytes");
        }
        return BitConverter.ToSingle(data);
    }

    internal static string ReadStringInternalDynamic(
        this Stream stream,
        Encoding encoding,
        char end
    )
    {
        int characterSize = encoding.GetByteCount("e");
        if (characterSize is not (1 or 2 or 4))
        {
            throw new ArgumentException(
                $"Unsupported character size: {characterSize}",
                nameof(encoding)
            );
        }

        string characterEnd = end.ToString(CultureInfo.InvariantCulture);
        const int maxCapacity = 65536;
        List<byte> bytes = new(128 * characterSize);
        byte[] buffer = new byte[characterSize];

        while (bytes.Count < maxCapacity * characterSize)
        {
            int read = stream.Read(buffer, 0, characterSize);
            if (read != characterSize)
            {
                throw new InvalidDataException("Unexpected end of stream while reading string");
            }
            if (encoding.GetString(buffer, 0, characterSize) == characterEnd)
            {
                break;
            }
            for (int j = 0; j < characterSize; j++)
            {
                bytes.Add(buffer[j]);
            }
        }

        if (bytes.Count == 0)
        {
            return "";
        }
        if (bytes.Count >= maxCapacity * characterSize)
        {
            throw new InvalidDataException("String exceeds maximum allowed length");
        }
        return encoding.GetString(bytes.ToArray());
    }

    public static string ReadStringUnicode(this Stream stream)
    {
        return stream.ReadStringInternalDynamic(Encoding.UTF8, '\0');
    }

    public static string ReadStringWide(this Stream stream)
    {
        return stream.ReadStringInternalDynamic(Encoding.Unicode, '\0');
    }
}
