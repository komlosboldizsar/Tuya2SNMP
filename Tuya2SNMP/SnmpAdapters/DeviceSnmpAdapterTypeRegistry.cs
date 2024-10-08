﻿using BToolbox.SNMP;
using Lextm.SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuya2SNMP.SnmpAdapters.General;

namespace Tuya2SNMP.SnmpAdapters
{
    internal class DeviceSnmpAdapterTypeRegistry
    {

        private static readonly Dictionary<string, DeviceSnmpAdapter.IFactory> _registeredFactories = new();

        private static readonly DeviceSnmpAdapter.IFactory[] _factoriesToRegister = new DeviceSnmpAdapter.IFactory[]
        {
            new SimpleRGB.Factory(),
            new SimpleRGBWW.Factory()
        };

        static DeviceSnmpAdapterTypeRegistry()
        {
            foreach (DeviceSnmpAdapter.IFactory factory in _factoriesToRegister)
                _registeredFactories.Add(factory.Type, factory);
        }

        public static DeviceSnmpAdapter GetInstance(string type, Device device, SnmpAgent snmpAgent)
        {
            if (!_registeredFactories.TryGetValue(type, out DeviceSnmpAdapter.IFactory adapterFactory))
                return null;
            return adapterFactory.GetInstance(device, snmpAgent);
        }

    }
}
