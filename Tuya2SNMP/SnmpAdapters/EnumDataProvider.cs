using BToolbox.Helpers;
using BToolbox.SNMP;
using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract class EnumDataProvider : DataProvider
    {

        public EnumDataProvider(int index, Dictionary<string, int> translations, int unknown = -1)
            : base(index)
        {
            _translations = translations;
            _translationsReverse = _translations.ReverseKeysValues();
            _unknown = unknown;
        }

        private readonly Dictionary<string, int> _translations;
        private readonly Dictionary<int, string> _translationsReverse;
        private int _unknown;

        public override ISnmpData Get(Device device)
        {
            int intValue;
            string stringValue = getStringValue(device);
            if ((stringValue == null) || !_translations.TryGetValue(stringValue, out intValue))
                intValue = _unknown;
            return new Integer32(intValue);
        }

        public override void Set(Device device, ISnmpData data)
        {
            if (data is not Integer32 intData)
                throw new SnmpErrorCodeException(ErrorCode.WrongType, "Value must be an integer.");
            if (!_translationsReverse.TryGetValue(intData.ToInt32(), out string stringValue))
                throw new SnmpErrorCodeException(ErrorCode.BadValue, "Value not in the enumeration range.");
            setStringValue(device, stringValue);
        }

        protected abstract string getStringValue(Device device);
        protected abstract void setStringValue(Device device, string stringValue);

    }
}
