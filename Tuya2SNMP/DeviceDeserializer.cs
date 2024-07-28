﻿using BToolbox.XmlDeserializer;
using BToolbox.XmlDeserializer.Attributes;
using BToolbox.XmlDeserializer.Context;
using BToolbox.XmlDeserializer.Exceptions;
using BToolbox.XmlDeserializer.Relations;
using System.Xml;

namespace Tuya2SNMP
{

    internal class DeviceDeserializer : ElementDeserializer<Device, Config>
    {

        public override string ElementName => ConfigTagNames.DEVICE;

        protected override Device createElement(XmlNode elementNode, DeserializationContext context, object parent)
        {
            Device device = new()
            {
                Name = elementNode.AttributeAsString(ATTR_NAME, context).Mandatory().NotEmpty().Get().Value,
                Type = elementNode.AttributeAsString(ATTR_TYPE, context).Mandatory().NotEmpty().Get().Value,
                IP = elementNode.AttributeAsString(ATTR_IP, context).Mandatory().NotEmpty().Get().Value,
                Key = elementNode.AttributeAsString(ATTR_KEY, context).Mandatory().NotEmpty().Get().Value,
                SnmpPort = (int)elementNode.AttributeAsInt(ATTR_SNMP_PORT, context).Mandatory().Min(1).Max(65535).Get().Value,
                SnmpCommunity = elementNode.AttributeAsString(ATTR_SNMP_COMMUNITY, context).Default("public").Get().Value
            };
            return device;
        }

        private const string ATTR_NAME = "name";
        private const string ATTR_TYPE = "type";
        private const string ATTR_IP = "ip";
        private const string ATTR_KEY = "key";
        private const string ATTR_SNMP_PORT = "snmp_port";
        private const string ATTR_SNMP_COMMUNITY = "snmp_community";

    }

}
