using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP
{
    internal class SnmpConfig
    {
        public int Port { get; init; }
        public string Community { get; init; }
        public IEnumerable<TrapTarget> TrapTargets { get; set; }
    }
}
