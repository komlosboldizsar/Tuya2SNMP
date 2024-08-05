using BToolbox.Helpers;
using BToolbox.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuya2SNMP.Tuya;

namespace Tuya2SNMP
{
    internal class Device
    {

        public Config Config { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }
        public TuyaProtocolVersion TuyaVersion { get; init; }
        public string IP { get; init; }
        public string Key { get; init; }
        public int SnmpPort { get; init; }
        public string SnmpCommunity { get; init; }

        private readonly List<IDpWatcher> _dpWatchers = new();

        internal void SubscribeDpWatcher(IDpWatcher dpWatcher)
            => _dpWatchers.Add(dpWatcher);

        private void NotifyDpWatchers(int dp, object value)
            => _dpWatchers.ForEach(dpw => dpw.DpChanged(this, dp, value));

        public interface IDpWatcher
        {
            void DpChanged(Device device, int dp, object value);
        }

        private DeviceSnmpAgent _agent;

        public void CreateSnmpAdapter()
        {
            _agent = new(SnmpPort, SnmpCommunity, Config.TrapSendingConfig);
            DeviceSnmpAdapter adapter = DeviceSnmpAdapterTypeRegistry.GetInstance(Type, this, _agent.ObjectStore);
            if (adapter == null)
                return;
        }

        public void StartSnmpAgent()
        {
            _agent.Start();
        }

        private ITuyaDevice _tuyaDevice;
        public readonly Dictionary<int, object> DPs = new();

        public void CreateTuyaAgent()
        {
            switch (TuyaVersion)
            {
                case TuyaProtocolVersion.V34:
                    _tuyaDevice = new TuyaDeviceV34(IP, Key)
                    {
                        PermanentConnection = true
                    };
                    break;
            }
            if (_tuyaDevice != null)
            {
                _tuyaDevice.DpsUpdated += dpsUpdatedHandler;
            }
        }

        private void dpsUpdatedHandler(ITuyaDevice tuyaDevice, Dictionary<int, object> dps)
        {
            DPs.AddOrReplaceValues(dps);
            dps.Foreach(kvp => NotifyDpWatchers(kvp.Key, kvp.Value));
        }

        public void StartTuyaAgent()
        {
            if (_tuyaDevice == null)
                return;
            if (_tuyaDevice.PermanentConnection)
                _ = Task.Run(() => _tuyaDevice.ConnectAsync());
        }

    }
}
