using BToolbox.SNMP;
using BToolbox.XmlDeserializer;
using BToolbox.XmlDeserializer.Attributes;
using BToolbox.XmlDeserializer.Context;
using BToolbox.XmlDeserializer.Exceptions;
using BToolbox.XmlDeserializer.Relations;
using System.Diagnostics;
using System.Xml;
using Tuya2SNMP.Tuya;

namespace Tuya2SNMP
{

    internal class SnmpConfigDeserializer : ElementDeserializer<SnmpConfig, Config>
    {

        public override string ElementName => ConfigTagNames.SNMPCONFIG;

        protected override SnmpConfig createElement(XmlNode elementNode, DeserializationContext context, object parent)
        {
            SnmpConfig snmpConfig = new()
            {
                Port = (int)elementNode.AttributeAsInt(ATTR_PORT, context).Mandatory().Min(1).Max(65535).Get().Value,
                Community = elementNode.AttributeAsString(ATTR_COMMUNITY, context).Mandatory().NotEmpty().Get().Value
            };
            snmpConfig.TrapTargets = trapTargetsDeserializer.ParseWithGivenParent(elementNode, context, out IRelationBuilder<Config> _, snmpConfig);
            return snmpConfig;
        }

        private static readonly SimpleListDeserializer<TrapTarget, Config> trapTargetsDeserializer = new(ConfigTagNames.SNMPCONFIG, new TrapTargetDeserializer());

        private const string ATTR_PORT = "port";
        private const string ATTR_COMMUNITY = "community";



    }

}
