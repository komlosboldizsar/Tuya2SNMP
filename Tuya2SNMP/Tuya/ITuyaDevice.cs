using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Tuya
{
    public interface ITuyaDevice
    {
        bool PermanentConnection { get; }
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task QueryDpsAsync(CancellationToken cancellationToken = default);
        Task SetDpAsync(int dp, object value, CancellationToken cancellationToken = default);
        Task SetDpsAsync(Dictionary<int, object> dps, CancellationToken cancellationToken = default);
        bool Connected { get; }
        event TuyaDeviceConnectionStateChangedHandler ConnectionEstablished;
        event TuyaDeviceConnectionStateChangedHandler ConnectionLost;
        event TuyaDeviceDpsUpdatedHandler DpsUpdated;
    }

    public delegate void TuyaDeviceConnectionStateChangedHandler(ITuyaDevice tuyaDevice, bool connected);
    public delegate void TuyaDeviceDpsUpdatedHandler(ITuyaDevice tuyaDevice, Dictionary<int, object> dps);

}
