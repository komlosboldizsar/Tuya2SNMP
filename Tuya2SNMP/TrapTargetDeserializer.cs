using BToolbox.SNMP;
using BToolbox.XmlDeserializer;
using BToolbox.XmlDeserializer.Attributes;
using BToolbox.XmlDeserializer.Context;
using System.Xml;

namespace Tuya2SNMP
{
    internal class TrapTargetDeserializer : ElementDeserializer<TrapTarget, Config>
    {

        public override string ElementName => ConfigTagNames.TRAPTARGET;

        protected override TrapTarget createElement(XmlNode elementNode, DeserializationContext context, object parent)
        {
            return new TrapTarget()
            {
                IP = elementNode.AttributeAsString(ATTR_IP, context).Get().Value,
                Port = (int)elementNode.AttributeAsInt(ATTR_PORT, context).Default(162).Min(1).Max(65535).Get().Value,
                Version = elementNode.AttributeAsEnum<TrapSendingConfig.TrapReceiverVersion>(ATTR_VERSION, context)
                                     .Translation("v1", TrapSendingConfig.TrapReceiverVersion.V1)
                                     .Translation("v2", TrapSendingConfig.TrapReceiverVersion.V2)
                                     .Get().Value,
                Community = elementNode.AttributeAsString(ATTR_COMMUNITY, context).Get().Value,
                SendMyIp = elementNode.AttributeAsBool(ATTR_SEND_MY_IP, context).Default(false).Get().Value
            };
        }

        private const string ATTR_IP = "ip";
        private const string ATTR_PORT = "port";
        private const string ATTR_VERSION = "version";
        private const string ATTR_COMMUNITY = "community";
        private const string ATTR_SEND_MY_IP = "send_my_ip";

    }
}
