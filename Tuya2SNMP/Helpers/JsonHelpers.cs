using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Helpers
{
    internal static class JsonHelpers
    {

        public static string UnformattedJson(string json)
            => JObject.Parse(json).ToString(Formatting.None);

    }
}
