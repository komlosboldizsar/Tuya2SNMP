using BToolbox.SNMP;
using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract class IntDataProvider : DataProvider
    {

        public IntDataProvider(int index, int unknown = -1, int? min = null, int? max = null)
            : base(index) 
        {
            _unknown = unknown;
            _min = min;
            _max = max;
        }

        private readonly int _unknown;
        private readonly int? _min;
        private readonly int? _max;

        public override ISnmpData Get(Device device) => new Integer32(getValue(device) ?? _unknown);

        public override void Set(Device device, ISnmpData data)
        {
            if (data is not Integer32 intData)
                throw new SnmpErrorCodeException(ErrorCode.WrongType, "Value must be an integer.");
            int intValue = intData.ToInt32();
            if ((_min != null) && (intValue < _min))
                throw new SnmpErrorCodeException(ErrorCode.BadValue, $"Value can't be less than {_min}.");
            if ((_max != null) && (intValue > _max))
                throw new SnmpErrorCodeException(ErrorCode.BadValue, $"Value can't be greater than {_min}.");
            setValue(device, intValue);
        }

        protected abstract int? getValue(Device device);
        protected abstract void setValue(Device device, int intValue);

    }
}
