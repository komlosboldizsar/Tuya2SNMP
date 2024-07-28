namespace Tuya2SNMP.Logger
{
    public record LogMessage(DateTime Timestamp, LogMessageSeverity Severity, string Message);
}
