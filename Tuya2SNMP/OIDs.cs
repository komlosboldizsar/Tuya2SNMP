using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP
{
    internal class OIDs
    {

        // .1.3.6.1.4.1
        // .59150 = komlosboldizsar
        // .6 = tuya2snmp
        public const string PRODUCT = "1.3.6.1.4.1.59150.6";

        public const string DEVICES = $"{PRODUCT}.1";

        public const int DEVICE_TRAPS = 9999;
        public const int CALCULATED_OFFSET = 10000;

    }
}
