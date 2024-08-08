namespace Tuya2SNMP.SnmpAdapters
{
    internal class SimpleBoolDataProvider : BoolDataProvider
    {

        public SimpleBoolDataProvider(int index, bool unknown = false)
            : base(index, unknown)
        { }

        public override int[] Dependencies => new int[] { Index };

        protected override bool? getValue(Device device) => device.GetDPbool(Index);
        protected override void setValue(Device device, bool boolValue) => device.SetDP(Index, boolValue);

    }
}
