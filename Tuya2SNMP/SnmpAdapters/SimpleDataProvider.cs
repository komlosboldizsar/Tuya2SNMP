using Lextm.SharpSnmpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract class SimpleDataProvider : DataProvider
    {
        public SimpleDataProvider(int index) => _index = index;
        private int _index;
        public override int Index => _index;
        public override int[] Dependencies => new int[] { _index };
    }
}
