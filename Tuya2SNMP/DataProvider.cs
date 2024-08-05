using Lextm.SharpSnmpLib;

namespace Tuya2SNMP
{
    internal abstract class DataProvider
    {
        public abstract int Index { get; }
        public abstract int[] Dependencies { get; }
        public abstract ISnmpData Get(Device device);
        public abstract void Set(Device device, ISnmpData data);
    }
}
