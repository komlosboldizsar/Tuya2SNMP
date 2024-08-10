using BToolbox.SNMP;
using BToolbox.XmlDeserializer;
using BToolbox.XmlDeserializer.Attributes;
using BToolbox.XmlDeserializer.Context;
using BToolbox.XmlDeserializer.Exceptions;
using BToolbox.XmlDeserializer.Relations;
using System.Xml;
using Tuya2SNMP.Tuya;

namespace Tuya2SNMP
{

    internal class DeviceDeserializer : ElementDeserializer<Device, Config>
    {

        public override string ElementName => ConfigTagNames.DEVICE;

        protected override Device createElement(XmlNode elementNode, DeserializationContext context, object parent)
        {
            Device device = new()
            {
                Index = (int)elementNode.AttributeAsInt(ATTR_INDEX, context).Mandatory().Min(1).Get().Value,
                Name = elementNode.AttributeAsString(ATTR_NAME, context).Mandatory().NotEmpty().Get().Value,
                Type = elementNode.AttributeAsString(ATTR_TYPE, context).Mandatory().NotEmpty().Get().Value,
                TuyaVersion = elementNode.AttributeAsEnum<TuyaProtocolVersion>(ATTR_VERSION, context)
                                     .Translation("34", TuyaProtocolVersion.V34)
                                     .Get().Value,
                IP = elementNode.AttributeAsString(ATTR_IP, context).Mandatory().NotEmpty().Get().Value,
                Key = elementNode.AttributeAsString(ATTR_KEY, context).Mandatory().NotEmpty().Get().Value
            };
            return device;
        }

        private const string ATTR_INDEX = "index";
        private const string ATTR_NAME = "name";
        private const string ATTR_TYPE = "type";
        private const string ATTR_VERSION = "version";
        private const string ATTR_IP = "ip";
        private const string ATTR_KEY = "key";

    }

}
