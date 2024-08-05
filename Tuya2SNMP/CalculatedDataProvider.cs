using Lextm.SharpSnmpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP
{
    internal abstract class CalculatedDataProvider : DataProvider
    {
        public CalculatedDataProvider(int index) => _index = OIDs.CALCULATED_OFFSET + index;
        private int _index;
        public override int Index => _index;
    }
}
