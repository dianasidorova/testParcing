using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace parce
{
    public class XmlToJsonParser
    {
        public static string ParseXmlToJson(string xmlFilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var xmlContent = File.ReadAllText(xmlFilePath, Encoding.GetEncoding("windows-1251"));

            var xdoc = XDocument.Parse(xmlContent);

            var document = new DocumentModel
            {
                Ppkp = xdoc.Root.Attribute("ppkp")?.Value,
                Ver = xdoc.Root.Attribute("ver")?.Value,
                Rev = xdoc.Root.Attribute("rev")?.Value,
                Zones = xdoc.Root.Element("zones")
                          ?.Elements("i")
                          ?.Where(x => x.Element("using")?.Value == "on")
                          .Select(x => new ZoneModel
                          {
                              Id = int.Parse(x.Attribute("id")?.Value ?? "0"),
                              Name = x.Element("name")?.Value,
                              Type = ConvertZoneType(x.Element("type")?.Value)
                          }).ToList(),

                Users = xdoc.Root.Element("users")
                          ?.Elements("i")
                          ?.Where(x => x.Element("using")?.Value == "on")
                          .Select(x => new UserModel
                          {
                              Id = int.Parse(x.Attribute("id")?.Value ?? "0"),
                              Using = x.Element("using")?.Value,
                              Name = x.Element("name")?.Value,
                          }).ToList(),

                Network = ParseNetwork(xdoc.Root.Element("network")),

                Modules = xdoc.Root.Element("modules")
                          ?.Elements("i")
                          ?.Where(x => x.Element("using")?.Value == "on")
                          ?.Where(x => x.Element("type")?.Value == "PUIZ")
                          .Select(x => new ModuleModel
                          {
                              Id = int.Parse(x.Attribute("id")?.Value ?? "0"),
                              Name = x.Element("name")?.Value,
                              Type = x.Element("type")?.Value
                          }).ToList(),


            };

            if (document.Zones == null || !document.Zones.Any())
            {
                throw new InvalidOperationException("Некоректний файл: немає активних зон.");
            }

            string jsonOutput = JsonConvert.SerializeObject(document, Formatting.Indented);
            return jsonOutput;
        }

        private static NetworkModel ParseNetwork(XElement networkElement)
        {
            if (networkElement == null) return null;

            var network = new NetworkModel
            {
                TestTime = networkElement.Element("PCN")?.Element("test_time")?.Value,
                Protocols = networkElement.Element("protocols")
                             ?.Elements("i")
                             .Select(p => new ProtocolModel
                             {
                                 Id = int.Parse(p.Attribute("id")?.Value ?? "0"),
                                 Type = p.Element("type")?.Value,
                                 SerKey = p.Element("ser_key")?.Value,
                                 HidKey = p.Element("hid_key")?.Value
                             }).ToList()
            };

            if (string.IsNullOrWhiteSpace(network.TestTime))
            {
                throw new InvalidOperationException("Некоректний файл: Поле 'TestTime' є порожнім.");
            }

            if (string.IsNullOrWhiteSpace(network.TestTime) && (network.Protocols == null || !network.Protocols.Any()))
            {
                throw new InvalidOperationException("Некоректний файл: немає протоколів.");
            }


            foreach (var protocol in network.Protocols)
            {
                if (string.IsNullOrWhiteSpace(protocol.SerKey) || string.IsNullOrWhiteSpace(protocol.HidKey))
                {
                    throw new InvalidOperationException($"Некоректний файл: Протокол з ID {protocol.Id} має порожній серійний або прихований номер.");
                }
            }

            return network;
        }

        private static string ConvertZoneType(string type)
        {
            return type switch
            {
                "manual" => "Ручний сповіщувач",
                "general" => "Автоматичний сповіщувач",
                _ => "Універсальний вхід"
            };
        }
    }
}
