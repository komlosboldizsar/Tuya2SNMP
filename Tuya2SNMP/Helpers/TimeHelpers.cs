using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Helpers
{
    internal static class TimeHelpers
    {

        public static long UnixSeconds
            => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

    }
}
