using Lextm.SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Devices.MiboxerFUTW
{
    internal class MiboxerFUT037W : DeviceSnmpAdapter
    {

        public class Factory : IFactory
        {
            public string Type { get; } = "miboxer_fut037w";
            public DeviceSnmpAdapter GetInstance(Device device, ObjectStore objectStore) => new MiboxerFUT037W(device, objectStore);
        }

        public MiboxerFUT037W(Device device, ObjectStore objectStore)
            : base(device, objectStore)
        { }

        // .1 = miboxerFUT037W
        public const int DEVICE_GROUP = 1;
        public override string OidBase => $"{OIDs.DEVICES}.{DEVICE_GROUP}";
        public override string TrapEnterpriseBase => $"{OidBase}.{OIDs.DEVICE_TRAPS}.1";

        protected override DataProvider[] DataProviders => new DataProvider[]
        {
            new BrightnessDataProvider(),
            new ColourDataProvider(),
            new SaturationDataProvider()
        };

    }
}
