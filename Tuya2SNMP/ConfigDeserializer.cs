using BToolbox.XmlDeserializer;
using BToolbox.XmlDeserializer.Context;
using BToolbox.XmlDeserializer.Exceptions;
using BToolbox.XmlDeserializer.Helpers;
using Tuya2SNMP.Logger;

namespace Tuya2SNMP
{
    internal class ConfigDeserializer
    {

        static ConfigDeserializer()
        {
            Deserializer = createDeserializer();
            RootDeserializer = new(Deserializer, rootDeserializerContextInitializer);
        }

        public Config LoadConfig(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    Config config = RootDeserializer.Deserialize(fileName, out DeserializationContext context, reportHandler);
                    int infoReportsCount = context.Reports.Count(r => r.Severity == DeserializationReportSeverity.Info);
                    if (infoReportsCount > 0)
                        LogDispatcher.I($"{infoReportsCount} verbose messages from configuration XML deserialization process.");
                    return config;
                }
                catch (DeserializationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DeserializationException("An exception was thrown while deserializing configuration XML!", ex);
                }
            }
            else
            {
                throw new DeserializationException($"Couldn't find {fileName}!");
            }
        }

        private void reportHandler(DeserializationContext context, IDeserializationReport report)
        {
            LogMessageSeverity severity = report.Severity switch
            {
                DeserializationReportSeverity.Info => LogMessageSeverity.Verbose,
                DeserializationReportSeverity.Warning => LogMessageSeverity.Warning,
                DeserializationReportSeverity.Error => LogMessageSeverity.Error,
                _ => LogMessageSeverity.Info
            };
            LogDispatcher.Log(severity, $"{report.XmlNode.GetPath()} :: {context.TranslateReportMessage(report)}");
        }

        public static readonly TypedCompositeDeserializer<Config, Config> Deserializer;
        public static readonly RootDeserializer<Config> RootDeserializer;

        private static TypedCompositeDeserializer<Config, Config> createDeserializer()
        {
            TypedCompositeDeserializer<Config, Config> configDeserializer = new(ConfigTagNames.ROOT, () => new Config());
            SimpleListDeserializer<Device, Config> switchesDeserializer = new(ConfigTagNames.DEVICES, new DeviceDeserializer());
            configDeserializer.Register(switchesDeserializer, (config, devices) => config.Devices = devices);
            return configDeserializer;
        }

        private static void rootDeserializerContextInitializer(DeserializationContext context)
        {
            context.RegisterTypeName<Device>("device");
        }

    }
}
