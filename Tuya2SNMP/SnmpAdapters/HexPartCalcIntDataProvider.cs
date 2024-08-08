using BToolbox.Helpers;
using BToolbox.SNMP;
using Lextm.SharpSnmpLib;
using System.Text;

namespace Tuya2SNMP.SnmpAdapters
{
    internal class HexPartCalcIntDataProvider : DataProvider
    {

        public HexPartCalcIntDataProvider(int indexThis, int indexBase, IEnumerable<HexStringDescriptor> descriptors, int descriptorIndex)
            : base(indexThis)
        {
            if ((descriptorIndex < 0) || (descriptorIndex >= descriptors.Count()))
                throw new ArgumentOutOfRangeException(nameof(descriptorIndex));
            _indexBase = indexBase;
            _descriptors = descriptors.ToArray();
            _descriptorIndex = descriptorIndex;
            _fullLength = _descriptors.Sum(d => d.Length);
            _offsets = new int[_descriptors.Length + 1];
            int offset = 0;
            int i = 0;
            for (; i <  _descriptors.Length; i++)
            {
                _offsets[i] = offset;
                offset += _descriptors[i].Length;
            }
            _offsets[i] = offset;
        }

        public override int[] Dependencies => new int[] { _indexBase };

        private readonly int _indexBase;
        private readonly HexStringDescriptor[] _descriptors;
        private readonly int _descriptorIndex;
        private readonly int _fullLength;
        private readonly int[] _offsets;

        public override ISnmpData Get(Device device)
        {
            string hexString = device.GetDPstring(_indexBase);
            int intValue = _descriptors[_descriptorIndex].Unknown;
            if ((hexString != null) && (hexString.Length == _fullLength))
            {
                try
                {
                    string hexStringPart = hexString.Substring(_offsets[_descriptorIndex], _descriptors[_descriptorIndex].Length);
                    intValue = int.Parse(hexStringPart, System.Globalization.NumberStyles.HexNumber);
                }
                catch (FormatException) { } // stays at 'Unknown'
            }
            return new Integer32(intValue);
        }

        public override void Set(Device device, ISnmpData data)
        {

            // Check
            if (data is not Integer32 intData)
                throw new SnmpErrorCodeException(ErrorCode.WrongType, "Value must be an integer.");
            int intValue = intData.ToInt32();
            int? min = _descriptors[_descriptorIndex].Min;
            if ((min != null) && (intValue < min))
                throw new SnmpErrorCodeException(ErrorCode.BadValue, $"Value can't be less than {min}.");
            int? max = _descriptors[_descriptorIndex].Max;
            if ((max != null) && (intValue > max))
                throw new SnmpErrorCodeException(ErrorCode.BadValue, $"Value can't be greater than {max}.");

            // Replace
            string hexString = device.GetDPstring(_indexBase);
            if ((hexString == null) || (hexString.Length != _fullLength))
                hexString = buildDefaultString();
            string hexStringPre = hexString[.._offsets[_descriptorIndex]];
            string hexStringPost = hexString[_offsets[_descriptorIndex+1]..^0];
            hexString = hexStringPre + intValue.ToString($"x{_descriptors[_descriptorIndex].Length}") + hexStringPost;
            device.SetDP(_indexBase, hexString);

        }

        private string buildDefaultString()
        {
            StringBuilder stringBuilder = new();
            foreach (HexStringDescriptor descriptor in _descriptors)
                stringBuilder.Append(descriptor.Unknown.ToString($"x{descriptor.Length}"));
            return stringBuilder.ToString();
        }

    }
}
