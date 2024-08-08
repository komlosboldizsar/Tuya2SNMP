namespace Tuya2SNMP.SnmpAdapters
{
    internal class SimpleIntDataProvider : IntDataProvider
    {

        public SimpleIntDataProvider(int index, int unknown = -1, int? min = null, int? max = null)
            : base(index, unknown, min, max)
        { }

        public override int[] Dependencies => new int[] { Index };

        protected override int? getValue(Device device) => device.GetDPinteger(Index);
        protected override void setValue(Device device, int intValue) => device.SetDP(Index, intValue);

    }
}
