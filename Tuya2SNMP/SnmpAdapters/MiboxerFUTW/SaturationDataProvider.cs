using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.SnmpAdapters.MiboxerFUTW
{
    internal class SaturationDataProvider : CalculatedDataProvider
    {

        public SaturationDataProvider() : base(1) { }

        public override int[] Dependencies => new int[] { DPs.COLOUR };

        public override ISnmpData Get(Device device) => new OctetString(device.Dp[DPs.COLOUR] as string);

        public override void Set(Device device, ISnmpData data)
            => device.Dp[DPs.COLOUR] = (data as OctetString).ToString();

    }
}
