using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Helpers
{
    internal static class ByteHelpers
    {
        public static IEnumerable<byte> BigEndian(IEnumerable<byte> byteSeq)
            => BitConverter.IsLittleEndian ? byteSeq.Reverse() : byteSeq;

        public static byte[] BigEndianArray(byte[] byteArray)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);
            return byteArray;
        }

        public static byte[] Take4BigEndian(this IEnumerable<byte> byteSeq, int offset = 0)
            => BigEndian(byteSeq.Skip(offset).Take(4)).ToArray();

        public static int ToInt(this IEnumerable<byte> byteSeq, int offset = 0)
            => BitConverter.ToInt32(byteSeq.Take4BigEndian(offset), 0);

        public static uint ToUInt(this IEnumerable<byte> byteSeq, int offset = 0)
            => BitConverter.ToUInt32(byteSeq.Take4BigEndian(offset), 0);

        public static byte[] ToBytesBigEndian(this int integer)
            => BigEndianArray(BitConverter.GetBytes(integer));

        public static byte[] ToBytesBigEndian(this uint uinteger)
            => BigEndianArray(BitConverter.GetBytes(uinteger));

        public static byte[] Part(this IEnumerable<byte> byteSeq, int offset, int length = 0)
            => byteSeq.Skip(offset).Take(length <= 0 ? byteSeq.Count() + length : length).ToArray();

        public static void WriteBytesBigEndian(this MemoryStream memoryStream, int integer, int offset = 0)
            => memoryStream.Write(integer.ToBytesBigEndian(), offset, 4);

        public static void WriteBytesBigEndian(this MemoryStream memoryStream, uint uinteger, int offset = 0)
            => memoryStream.Write(uinteger.ToBytesBigEndian(), offset, 4);

    }
}
