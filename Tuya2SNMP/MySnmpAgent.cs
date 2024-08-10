using BToolbox.Model;
using BToolbox.SNMP;
using Tuya2SNMP.SnmpAdapters;

namespace Tuya2SNMP
{
    internal class MySnmpAgent : SnmpAgent
    {

        public MySnmpAgent(SnmpConfig snmpConfig)
            : base(snmpConfig.Port, snmpConfig.Community, snmpConfig.Community, createTrapSendingConfig(snmpConfig))
        { }

        private static TrapSendingConfig createTrapSendingConfig(SnmpConfig snmpConfig)
        {
            TrapSendingConfig trapSendingConfig = new();
            snmpConfig.TrapTargets.Foreach(tt => trapSendingConfig.AddReceiver(tt.IP, tt.Port, tt.Version, tt.Community, null, tt.SendMyIp));
            return trapSendingConfig;
        }

        public override string OID_BASE => OIDs.PRODUCT;


    }
}
