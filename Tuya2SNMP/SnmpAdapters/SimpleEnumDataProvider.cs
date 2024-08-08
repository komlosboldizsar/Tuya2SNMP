namespace Tuya2SNMP.SnmpAdapters
{
    internal class SimpleEnumDataProvider : EnumDataProvider
    {

        public SimpleEnumDataProvider(int index, Dictionary<string, int> translations, int unknown = -1)
            : base(index, translations, unknown)
        { }

        public override int[] Dependencies => new int[] { Index };

        protected override string getStringValue(Device device) => device.GetDPstring(Index);
        protected override void setStringValue(Device device, string stringValue) => device.SetDP(Index, stringValue);

    }
}
