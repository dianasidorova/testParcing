using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parce
{
    public class DocumentModel
    {
        public string Ppkp { get; set; }
        public string Ver { get; set; }
        public string Rev { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ZoneModel> Zones { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<UserModel> Users { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ModuleModel> Modules { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NetworkModel PCN { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ProtocolModel> Protocols { get; set; }
    }
}
