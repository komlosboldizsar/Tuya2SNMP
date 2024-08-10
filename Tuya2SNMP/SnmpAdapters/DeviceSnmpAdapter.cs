using BToolbox.Helpers;
using BToolbox.Model;
using BToolbox.SNMP;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Tuya2SNMP.SnmpAdapters
{
    internal abstract partial class DeviceSnmpAdapter : Device.IDpWatcher
    {

        public interface IFactory
        {
            string Type { get; }
            DeviceSnmpAdapter GetInstance(Device device, SnmpAgent snmpAgent);
        }

        private SnmpAgent _snmpAgent;
        private readonly Dictionary<int, List<DpSnmpObject>> _objectDependencies = new();

        public abstract int TypeNumber { get; }
        protected abstract DataProvider[] DataProviders { get; }

        private string OidBase => $"{OIDs.DEVICES}.{TypeNumber}";
        private string VarOidBase => $"{OidBase}.1.1";
        private const int INDEXER_VAR_OID = 0;
        private string TrapEnterpriseBase => $"{OidBase}.{OIDs.DEVICE_TRAPS}";

        public DeviceSnmpAdapter(Device device, SnmpAgent snmpAgent)
        {
            _snmpAgent = snmpAgent;
            foreach (DataProvider dataProvider in DataProviders)
            {
                DpSnmpObject snmpObject = new(VarOidBase, device, dataProvider);
                _snmpAgent.ObjectStore.Add(snmpObject);
                foreach (int dependency in dataProvider.Dependencies)
                    _objectDependencies.GetAnyway(dependency).Add(snmpObject);
            }
            device.SubscribeDpWatcher(this);
        }

        public void DpChanged(Device device, int dp, object value)
        {
            if (!_objectDependencies.TryGetValue(dp, out List<DpSnmpObject> varGens))
                return;
            List<Variable> variables = new()
            {
                new(new ObjectIdentifier($"{VarOidBase}.{INDEXER_VAR_OID}"), new Integer32(device.Index))
            };
            variables.AddRange(varGens.Select(vg => vg.Variable));
            _snmpAgent.SendTraps($"dpchange:{dp}", new TrapEnterprise(TrapEnterpriseBase, dp), variables);
        }

    }
}
