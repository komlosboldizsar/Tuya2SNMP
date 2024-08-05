using BToolbox.SNMP;

namespace Tuya2SNMP
{
    internal class Config
    {
        public IList<Device> Devices { get; set; }
        public TrapSendingConfig TrapSendingConfig { get; set; }
    }
}
