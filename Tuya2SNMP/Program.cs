using BToolbox.XmlDeserializer.Exceptions;
using CommandLine;
using Tuya2SNMP.Logger;

namespace Tuya2SNMP
{
    internal static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Arguments parsedArguments = Parser.Default.ParseArguments<Arguments>(args).Value;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _ = new FileLogger(parsedArguments.VeryVerbose ? null : LogMessageSeverity.Verbose);
            LogDispatcher.I("Program started.");
            Config config = null;
            string parsingError = null;
            try
            {
                LogDispatcher.I("Loading configuration...");
                config = (new ConfigDeserializer()).LoadConfig(parsedArguments.ConfigFile ?? DEFAULT_CONFIG_FILE);
            }
            catch (DeserializationException e)
            {
                parsingError = e.Message;
                LogDispatcher.E("XML configuration parsing error: " + e.Message);
                if (e.InnerException != null)
                    LogDispatcher.E("Inner exception: " + e.InnerException.Message);
            }
            LogDispatcher.I("Starting GUI...");
            MainForm mainForm = new(config, parsingError, parsedArguments.Hidden);
            Application.Run(mainForm);
        }

        private const string DEFAULT_CONFIG_FILE = "config.xml";

    }
}