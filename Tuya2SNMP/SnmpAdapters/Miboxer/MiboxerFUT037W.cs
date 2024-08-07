﻿using BToolbox.SNMP;
using Lextm.SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.SnmpAdapters.MiboxerFUTW
{
    internal class MiboxerFUT037W : DeviceSnmpAdapter
    {

        public class Factory : IFactory
        {
            public string Type { get; } = "miboxer_fut037w";
            public DeviceSnmpAdapter GetInstance(Device device, SnmpAgent snmpAgent) => new MiboxerFUT037W(device, snmpAgent);
        }

        public MiboxerFUT037W(Device device, SnmpAgent snmpAgent)
            : base(device, snmpAgent)
        { }

        // .1 = miboxerFUT037W
        public const int DEVICE_GROUP = 1;
        public override string OidBase => $"{OIDs.DEVICES}.{DEVICE_GROUP}";
        public override string TrapEnterpriseBase => $"{OidBase}.{OIDs.DEVICE_TRAPS}.1";

        protected override DataProvider[] DataProviders => new DataProvider[]
        {
            new SimpleBoolDataProvider(DP_SWITCH),
            new SimpleEnumDataProvider(DP_WORKMODE, workmodes, -1),
            new SimpleIntDataProvider(DP_BRIGHT, -1, 10, 1000),
            new SimpleIntDataProvider(DP_TEMP, -1, 0, 1000),
            new SimpleHexStringDataProvider(DP_COLOUR, new HexStringDataProvider.HexStringDescriptor[]
            {
                new(4, 0, 359),
                new(4, 10, 1000),
                new(4, 10, 1000)
            })
        };

        public const int DP_SWITCH = 20;
        public const int DP_WORKMODE = 21;
        public const int DP_BRIGHT = 22;
        public const int DP_TEMP = 23;
        public const int DP_COLOUR = 24;

        private readonly Dictionary<string, int> workmodes = new()
        {
            { "white", 0 },
            { "colour", 1 },
            { "scene", 2 },
            { "music", 3 },
        };

    }
}
