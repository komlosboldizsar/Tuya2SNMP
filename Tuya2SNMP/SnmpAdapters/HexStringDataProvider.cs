using BToolbox.Helpers;
using BToolbox.SNMP;
using Lextm.SharpSnmpLib;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract class HexStringDataProvider : DataProvider
    {

        public HexStringDataProvider(int index, IEnumerable<HexStringDescriptor> descriptors)
            : base(index)
        {
            _descriptors.AddRange(descriptors);
            _length = descriptors.Sum(d => d.Length);
        }

        private readonly List<HexStringDescriptor> _descriptors = new();
        private readonly int _length;

        public override ISnmpData Get(Device device) => new OctetString(getStringValue(device));

        public override void Set(Device device, ISnmpData data)
        {
            if (data is not OctetString stringData)
                throw new SnmpErrorCodeException(ErrorCode.WrongType, "Value must be a string.");
            string stringValue = stringData.ToString();
            if (stringValue.Length != _length)
                throw new SnmpErrorCodeException(ErrorCode.BadValue, $"Value must be {_length} hexadecimal characters.");
            int start = 0;
            int i = 0;
            foreach (HexStringDescriptor descriptor in _descriptors)
            {
                string positionInfo() => $"Value at position #{i} (characters from {start} to {start + descriptor.Length - 1})";
                string part = stringValue.Substring(start, descriptor.Length);
                try
                {
                    int partInt = int.Parse(part, System.Globalization.NumberStyles.HexNumber);
                    if ((descriptor.Min != null) && (partInt < descriptor.Min))
                        throw new SnmpErrorCodeException(ErrorCode.BadValue, $"{positionInfo()} can't be less than {descriptor.Min}.");
                    if ((descriptor.Max != null) && (partInt > descriptor.Max))
                        throw new SnmpErrorCodeException(ErrorCode.BadValue, $"{positionInfo()} can't be greater than {descriptor.Max}.");
                }
                catch (FormatException)
                {
                    throw new SnmpErrorCodeException(ErrorCode.BadValue, $"{positionInfo()} is not a hexadecimal number.");
                }
                start += descriptor.Length;
                i++;
            }
            setStringValue(device, stringValue);
        }

        protected abstract string getStringValue(Device device);
        protected abstract void setStringValue(Device device, string stringValue);

    }
}
