using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.Devices.MiboxerFUTW
{
    internal class BrightnessDataProvider : SimpleDataProvider
    {

        public BrightnessDataProvider() : base(DPs.BRIGHT) { }

        public override int[] Dependencies => new int[] { DPs.BRIGHT };

        public override ISnmpData Get(Device device) => new Integer32((int)device.Dp[DPs.BRIGHT]);

        public override void Set(Device device, ISnmpData data)
            => device.Dp[DPs.BRIGHT] = (data as Integer32).ToInt32();

    }
}
