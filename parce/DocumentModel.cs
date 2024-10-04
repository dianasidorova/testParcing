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
        public List<ZoneModel> Zones { get; set; }
        public List<UserModel> Users { get; set; }
        public List<ModuleModel> Modules { get; set; }
        public NetworkModel Network { get; set; } 
    }
}
