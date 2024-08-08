using BToolbox.SNMP;
using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract class BoolDataProvider : DataProvider
    {

        public BoolDataProvider(int index, bool unknown = false)
            : base(index) 
        {
            _unknown = unknown;
        }

        private readonly bool _unknown;

        private const int VALUE_TRUE = 1;
        private const int VALUE_FALSE = 2;

        public override ISnmpData Get(Device device) => new Integer32((getValue(device) ?? _unknown) ? VALUE_TRUE : VALUE_FALSE);

        public override void Set(Device device, ISnmpData data)
        {
            string errorMessage = "Value must be a TruthValue: an integer between 1 and 2.";
            if (data is not Integer32 intData)
                throw new SnmpErrorCodeException(ErrorCode.WrongType, errorMessage);
            int intValue = intData.ToInt32();
            if ((intValue < VALUE_TRUE) || (intValue > VALUE_FALSE))
                throw new SnmpErrorCodeException(ErrorCode.BadValue, errorMessage);
            setValue(device, intValue == VALUE_TRUE);
        }

        protected abstract bool? getValue(Device device);
        protected abstract void setValue(Device device, bool boolValue);

    }
}
