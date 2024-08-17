using System.Text;

namespace Tuya2SNMP.Tuya
{
    /// <summary>
    /// Response from local Tuya device.
    /// </summary>
    public class TuyaLocalResponse
    {

        public TuyaCommand Command { get; }
        public uint SequenceNumber { get; }
        public int ReturnCode { get; }
        public byte[] ByteArray { get; }
        public string JSON { get; }

        internal TuyaLocalResponse(TuyaCommand command, uint sequenceNumber, int returnCode, byte[] byteArray)
        {
            Command = command;
            SequenceNumber = sequenceNumber;
            ReturnCode = returnCode;
            ByteArray = byteArray;
            if (byteArray.Length > 0)
            {
                var str = Encoding.UTF8.GetString(byteArray);
                if (str.StartsWith("{") && str.EndsWith("}"))
                {
                    JSON = str;
                }
            }
        }

    }
}
