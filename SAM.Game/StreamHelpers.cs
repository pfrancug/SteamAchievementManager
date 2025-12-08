/*
 * Copyright (c) 2025 Piotr Francug - HotCode
 * Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SAM.Game
{
    internal static class StreamHelpers
    {
        public static byte ReadValueU8(this Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        public static int ReadValueS32(this Stream stream)
        {
            var data = new byte[4];
            int read = stream.Read(data, 0, 4);
            if (read != 4)
            {
                throw new InvalidDataException($"Failed to read 4 bytes, only read {read}");
            }
            return BitConverter.ToInt32(data, 0);
        }

        public static uint ReadValueU32(this Stream stream)
        {
            var data = new byte[4];
            int read = stream.Read(data, 0, 4);
            if (read != 4)
            {
                throw new InvalidDataException($"Failed to read 4 bytes, only read {read}");
            }
            return BitConverter.ToUInt32(data, 0);
        }

        public static ulong ReadValueU64(this Stream stream)
        {
            var data = new byte[8];
            int read = stream.Read(data, 0, 8);
            if (read != 8)
            {
                throw new InvalidDataException($"Failed to read 8 bytes, only read {read}");
            }
            return BitConverter.ToUInt64(data, 0);
        }

        public static float ReadValueF32(this Stream stream)
        {
            var data = new byte[4];
            int read = stream.Read(data, 0, 4);
            if (read != 4)
            {
                throw new InvalidDataException($"Failed to read 4 bytes, only read {read}");
            }
            return BitConverter.ToSingle(data, 0);
        }

        internal static string ReadStringInternalDynamic(
            this Stream stream,
            Encoding encoding,
            char end
        )
        {
            int characterSize = encoding.GetByteCount("e");
            if (characterSize != 1 && characterSize != 2 && characterSize != 4)
            {
                throw new ArgumentException(
                    $"Unsupported character size: {characterSize}",
                    nameof(encoding)
                );
            }
            string characterEnd = end.ToString(CultureInfo.InvariantCulture);
            const int initialCapacity = 128;
            const int maxCapacity = 65536;
            List<byte> bytes = new(initialCapacity * characterSize);
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

        public static string ReadStringAscii(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.ASCII, '\0');
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
}
