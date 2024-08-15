using BToolbox.Helpers;
using BToolbox.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuya2SNMP.SnmpAdapters;
using Tuya2SNMP.Tuya;

namespace Tuya2SNMP
{
    internal class Device
    {

        public Config Config { get; set; }

        #region Base data
        public int Index { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }
        public TuyaProtocolVersion TuyaVersion { get; init; }
        public string IP { get; init; }
        public string Key { get; init; }
        #endregion

        #region DPs, DP watching
        private readonly List<IDpWatcher> _dpWatchers = new();

        internal void SubscribeDpWatcher(IDpWatcher dpWatcher)
            => _dpWatchers.Add(dpWatcher);

        private void NotifyDpWatchers(int dp, object value)
            => _dpWatchers.ForEach(dpw => dpw.DpChanged(this, dp, value));

        public interface IDpWatcher
        {
            void DpChanged(Device device, int dp, object value);
        }

        private void dpsUpdatedHandler(ITuyaDevice tuyaDevice, Dictionary<int, object> dps)
        {
            foreach (KeyValuePair<int, object> dp in dps) // replace and recast
            {
                object newValue = dp.Value.IsNumericType() ? (int)(long)dp.Value : dp.Value;
                dps[dp.Key] = newValue;
                _dps[dp.Key] = newValue;
            }
            dps.Foreach(kvp => NotifyDpWatchers(kvp.Key, kvp.Value));
        }

        public void SetDP(int dp, object value) => _ = TuyaDevice.SetDpAsync(dp, value);
        public bool GetDP(int dp, out object value) => _dps.TryGetValue(dp, out value);

        public bool? GetDPbool(int dp)
            => (GetDP(dp, out object value) && (value is bool boolValue)) ? boolValue : null;

        public int? GetDPinteger(int dp)
            => (GetDP(dp, out object value) && (value is int intValue)) ? intValue : null;

        public string GetDPstring(int dp)
            => (GetDP(dp, out object value) && (value is string strValue)) ? strValue : null;
        #endregion

        #region SNMP
        private MySnmpAgent _snmpAgent;
        public DeviceSnmpAdapter Adapter { get; private set; }

        public void SetSnmpAgent(MySnmpAgent snmpAgent)
        {
            if (_snmpAgent != null)
                throw new Exception("SNMP agent already set for this device.");
            _snmpAgent = snmpAgent;
            Adapter = DeviceSnmpAdapterTypeRegistry.GetInstance(Type, this, _snmpAgent);
        }
        #endregion

        #region Tuya
        public ITuyaDevice TuyaDevice { get; private set; }
        private readonly Dictionary<int, object> _dps = new();

        public void CreateTuyaAgent()
        {
            switch (TuyaVersion)
            {
                case TuyaProtocolVersion.V34:
                    TuyaDevice = new TuyaDeviceV34(IP, Key)
                    {
                        PermanentConnection = true
                    };
                    break;
            }
            if (TuyaDevice != null)
            {
                TuyaDevice.ConnectionEstablished += _tuyaDevice_ConnectionEstablished;
                TuyaDevice.DpsUpdated += dpsUpdatedHandler;
            }
        }

        private void _tuyaDevice_ConnectionEstablished(ITuyaDevice tuyaDevice, bool connected)
        {
            _ = tuyaDevice.QueryDpsAsync();
        }

        public void StartTuyaAgent()
        {
            if (TuyaDevice == null)
                return;
            if (TuyaDevice.PermanentConnection)
                _ = Task.Run(() => TuyaDevice.ConnectAsync());
        }
        #endregion

    }
}
