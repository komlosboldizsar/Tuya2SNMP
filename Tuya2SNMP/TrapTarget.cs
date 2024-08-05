using BToolbox.SNMP;

namespace Tuya2SNMP
{
    internal class TrapTarget
    {
        public string IP { init; get; }
        public int Port { init; get; }
        public TrapSendingConfig.TrapReceiverVersion Version { init; get; }
        public string Community { init; get; }
        public bool SendMyIp { init; get; }
    }
}
