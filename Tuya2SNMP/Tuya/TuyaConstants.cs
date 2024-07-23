using com.clusterrr.TuyaNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Tuya
{
    internal abstract class TuyaConstants
    {

        public static readonly byte[] PREFIX = new byte[] { 0, 0, 0x55, 0xAA };
        public static readonly byte[] SUFFIX = { 0, 0, 0xAA, 0x55 };
    }
}
