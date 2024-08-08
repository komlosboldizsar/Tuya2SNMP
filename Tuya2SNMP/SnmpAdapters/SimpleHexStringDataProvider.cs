namespace Tuya2SNMP.SnmpAdapters
{
    internal class SimpleHexStringDataProvider : HexStringDataProvider
    {

        public SimpleHexStringDataProvider(int index, IEnumerable<HexStringDescriptor> descriptors)
            : base(index, descriptors)
        { }

        public override int[] Dependencies => new int[] { Index };

        protected override string getStringValue(Device device) => device.GetDPstring(Index);
        protected override void setStringValue(Device device, string stringValue) => device.SetDP(Index, stringValue);

    }
}
