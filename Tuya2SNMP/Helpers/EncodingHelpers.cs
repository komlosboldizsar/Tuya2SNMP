using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Helpers
{
    internal static class EncodingHelpers
    {

        public static byte[] UTF8toBytes(this string str)
            => Encoding.UTF8.GetBytes(str);

        public static string ToUTF8(this byte[] byteArray)
            => Encoding.UTF8.GetString(byteArray);

    }
}
