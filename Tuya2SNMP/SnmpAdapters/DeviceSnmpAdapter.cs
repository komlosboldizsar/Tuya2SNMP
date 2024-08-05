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
            DeviceSnmpAdapter GetInstance(Device device, ObjectStore objectStore);
        }

        private SnmpAgent _snmpAgent;
        private readonly Dictionary<int, List<DpSnmpObject>> _objectDependencies = new();

        public abstract string OidBase { get; }
        public abstract string TrapEnterpriseBase { get; }
        protected abstract DataProvider[] DataProviders { get; }

        public DeviceSnmpAdapter(Device device, ObjectStore objectStore)
        {
            foreach (DataProvider dataProvider in DataProviders)
            {
                DpSnmpObject snmpObject = new(OidBase, dataProvider);
                objectStore.Add(snmpObject);
                foreach (int dependency in dataProvider.Dependencies)
                    _objectDependencies.GetAnyway(dependency).Add(snmpObject);
            }
            device.SubscribeDpWatcher(this);
        }

        public void DpChanged(Device device, int dp, object value)
        {
            if (!_objectDependencies.TryGetValue(dp, out List<DpSnmpObject> varGens))
                return;
            IList<Variable> variables = varGens.Select(vg => vg.Variable).ToList();
            _snmpAgent.SendTraps($"dpchange:{dp}", new TrapEnterprise(TrapEnterpriseBase, dp), variables);
        }

    }
}
