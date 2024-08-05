using BToolbox.SNMP;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Pipeline;

namespace Tuya2SNMP
{
    internal class DpSnmpObject : ScalarObject
    {

        private readonly Device _device;
        private readonly DataProvider _datProvider;

        public DpSnmpObject(string oidBase, DataProvider dataProvider)
            : base(new ObjectIdentifier($"{oidBase}.{dataProvider.Index}"))
            => _datProvider = dataProvider;

        public override ISnmpData Data
        {
            get => _datProvider.Get(_device);
            set => _datProvider.Set(_device, value);
        }

    }
}
