using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParsingPrimeA
{
    public class XmlToJsonParser
    {
        public static string ParseXmlToJson(string xmlFilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var xmlContent = File.ReadAllText(xmlFilePath, Encoding.GetEncoding("windows-1251"));

            var xdoc = XDocument.Parse(xmlContent);

            var config = new ConfigModel
            {
                Model = xdoc.Root.Attribute("model")?.Value,
                Version = xdoc.Root.Attribute("ver")?.Value,
                
                Users = xdoc.Root.Element("modules")
                            .Element("FxUsersModule")
                             ?.Elements("i")
                             .Select(u => new UserModel
                             {
                                 Id = int.Parse(u.Attribute("id")?.Value ?? "0") + 1 ,
                                 Name = u.Element("name")?.Value,
                             }).ToList(),

                ObjectNumber = xdoc.Root.Element("modules")?
                   .Elements("FxNetworkModule")
                   .Elements("i")
                   .Select(x => x.Element("object_number")?.Value)
                   .FirstOrDefault(),

                HiddenNumber = xdoc.Root.Element("modules")?
                   .Elements("FxNetworkModule")
                   .Elements("i")
                   .Select(x => x.Element("hidden_number")?.Value)
                   .FirstOrDefault(),

                TestPeriod = xdoc.Root.Element("modules")?
                   .Elements("FxNetworkModule")
                   .Elements("i")
                   .Select(x => x.Element("test_period")?.Value)
                   .FirstOrDefault(),

                Zones = xdoc.Root.Element("modules")
                          .Element("FxZonesModule")
                          ?.Elements("i")
                          .Select(x => new ZoneModel
                          {
                              Id = int.Parse(x.Attribute("id")?.Value ?? "0") + 1,
                              Name = x.Element("name")?.Value
                          }).ToList(),


            };

            if (config.Zones == null || !config.Zones.Any())
            {
                throw new InvalidOperationException("Некоректний файл: немає активних зон.");
            }

            string jsonOutput = JsonConvert.SerializeObject(config, Formatting.Indented);
            return jsonOutput;
        }


    }
}
