using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract class DataProvider
    {
        public DataProvider(int index) => Index = index;
        public int Index { get; init; }
        public abstract int[] Dependencies { get; }
        public abstract ISnmpData Get(Device device);
        public abstract void Set(Device device, ISnmpData data);
    }
}
