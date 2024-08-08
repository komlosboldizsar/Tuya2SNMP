using BToolbox.SNMP;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Pipeline;

namespace Tuya2SNMP.SnmpAdapters
{
    internal class DpSnmpObject : ScalarObject
    {

        private readonly Device _device;
        private readonly DataProvider _dataProvider;

        public DpSnmpObject(string oidBase, Device device, DataProvider dataProvider)
            : base(new ObjectIdentifier($"{oidBase}.{dataProvider.Index}.0"))
        {
            _device = device;
            _dataProvider = dataProvider;
        }

        public override ISnmpData Data
        {
            get => _dataProvider.Get(_device);
            set => _dataProvider.Set(_device, value);
        }

    }
}
