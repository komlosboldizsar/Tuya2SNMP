using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.Devices.MiboxerFUTW
{
    internal class ColourDataProvider : SimpleDataProvider
    {

        public ColourDataProvider() : base(DPs.COLOUR) { }

        public override int[] Dependencies => new int[] { DPs.COLOUR };

        public override ISnmpData Get(Device device) => new OctetString(device.Dp[DPs.COLOUR] as string);

        public override void Set(Device device, ISnmpData data)
            => device.Dp[DPs.COLOUR] = (data as OctetString).ToString();

    }
}
