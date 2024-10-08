using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingMix
{
    public class BaseModel
    {
        public string Model { get; set; }
        public string Version { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Revision { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ZoneModel> Zones { get; set; }

        public string ObjectNumber { get; set; }
        public string HiddenNumber { get; set; }
        public string TestPeriod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ModuleModel> Modules { get; set; }

    }
}
