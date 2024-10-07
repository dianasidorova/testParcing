using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingPrimeA
{
    public class ConfigModel
    {
        public string Model { get; set; }
        public string Version { get; set; }

        public string ObjectNumber { get; set; }
        public string HiddenNumber { get; set; }
        public string TestPeriod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ZoneModel> Zones { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<UserModel> Users { get; set; }
    }
}
