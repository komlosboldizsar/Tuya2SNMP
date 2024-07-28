using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP
{
    internal class Device
    {

        public string Name { get; init; }
        public string Type { get; init; }
        public string IP { get; init; }
        public string Key { get; init; }
        public int SnmpPort { get; init; }
        public string SnmpCommunity { get; init; }


    }
}
