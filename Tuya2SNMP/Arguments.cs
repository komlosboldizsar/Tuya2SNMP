using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP
{
    internal class Arguments
    {

        [Option('v', "verbose")]
        public bool VeryVerbose { get; set; }

        [Option('h', "hidden")]
        public bool Hidden { get; set; }

        [Value(0)]
        public string ConfigFile { get; set; }

    }
}
