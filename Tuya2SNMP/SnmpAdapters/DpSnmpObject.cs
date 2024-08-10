using BToolbox.SNMP;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Pipeline;

namespace Tuya2SNMP.SnmpAdapters
{
    internal class DpSnmpObject : ScalarObject
    {

        private readonly Device _device;
        private readonly DataProvider _dataProvider;
        private readonly ObjectIdentifier _oidWithoutIndexer;

        public DpSnmpObject(string oidBase, Device device, DataProvider dataProvider)
            : base(new ObjectIdentifier($"{oidWithoutIndexer(oidBase, dataProvider)}.{device.Index}"))
        {
            _device = device;
            _dataProvider = dataProvider;
            _oidWithoutIndexer = new ObjectIdentifier(oidWithoutIndexer(oidBase, dataProvider));
        }

        private static string oidWithoutIndexer(string oidBase, DataProvider dataProvider)
            => $"{oidBase}.{dataProvider.Index}";

        public override ISnmpData Data
        {
            get => _dataProvider.Get(_device);
            set => _dataProvider.Set(_device, value);
        }

        public Variable VariableWithoutIndexer => new(_oidWithoutIndexer, Data);

    }
}
