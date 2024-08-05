using BToolbox.SNMP;

namespace Tuya2SNMP.SnmpAdapters
{
    internal class DeviceSnmpAgent : SnmpAgent
    {

        public DeviceSnmpAgent(int port, string community, TrapSendingConfig trapSendingConfig)
            : base(port, community, community, trapSendingConfig)
        { }

        public override string OID_BASE => OIDs.PRODUCT;


    }
}
