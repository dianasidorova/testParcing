using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParsingMix
{
    public class XmlToJsonParser
    {
        private readonly Dictionary<string, Func<XDocument, BaseModel>> parsers;
        public BaseModel ParseXmlFile(string xmlFilePath)
        {
            XDocument xdoc = XDocument.Load(xmlFilePath);

            string structureKey = DetermineStructureKey(xdoc);

            if (structureKey != null && parsers.ContainsKey(structureKey))
            {
                return parsers[structureKey](xdoc);
            }
            else
            {
                throw new InvalidOperationException("Невідома структура XML файла");
            }
        }
        private string DetermineStructureKey(XDocument xdoc)
        {
            var root = xdoc.Root;

            if (HasStructure(root, new[] { "zones", "users", "outputs" }))
            {
                return "TypePrime16";
            }

            if (HasStructure(root.Element("modules"), new[] { "FxNetworkModule" }))
            {
                return "TypePrimeA";
            }

            return null;
        }
        public XmlToJsonParser()
        {
            parsers = new Dictionary<string, Func<XDocument, BaseModel>>
            {
                { "TypePrimeA", ParseXmlToJsonPrime_A },  
                { "TypePrime16", ParseXmlToJson }   
            };
        }

        private BaseModel ParseXmlToJson(XDocument document)
        {
            var doc = new BaseModel
            {
                Model = document.Root.Attribute("ppkp")?.Value,

                Zones = document.Root.Element("zones")
                          ?.Elements("i")
                          ?.Where(x => x.Element("using")?.Value == "on")
                          .Select(x => new ZoneModel
                          {
                              Id = int.Parse(x.Attribute("id")?.Value ?? "0"),
                              Name = x.Element("name")?.Value,
                              TypeZone = ConvertZoneType(x.Element("type")?.Value),
                          }).ToList(),

                ObjectNumber = document.Root.Element("network")?
                  .Elements("protocols")
                  .Elements("i")
                  .Select(x => x.Element("ser_key")?.Value)
                  .FirstOrDefault(),

                HiddenNumber = document.Root.Element("network")?
                  .Elements("protocols")
                  .Elements("i")
                  .Select(x => x.Element("hid_key")?.Value)
                  .FirstOrDefault(),

                TestPeriod = ParseNetwork(document.Root.Element("network")),
            };

            var modules = document.Root.Element("modules")
                         ?.Elements("i")
                         ?.Where(x => x.Element("using")?.Value == "on")
                         ?.Where(x => x.Element("type")?.Value == "PUIZ")
                         .Select(x => new ModuleModel
                         {
                             Id = int.Parse(x.Attribute("id")?.Value ?? "0"),
                             Name = x.Element("name")?.Value,
                             Type = x.Element("type")?.Value
                         }).ToList();

            if (modules != null && modules.Any())
            {
                doc.Modules = modules;
            }

            if (doc.Zones == null || !doc.Zones.Any())
            {
                throw new InvalidOperationException("Некоректний файл: немає активних зон.");
            }

            if (string.IsNullOrWhiteSpace(doc.ObjectNumber) || string.IsNullOrWhiteSpace(doc.HiddenNumber))
            {
                throw new InvalidOperationException($"Некоректний файл: Протокол має порожній серійний або прихований номер.");
            }

            if (int.Parse(doc.ObjectNumber) == 0 || int.Parse(doc.HiddenNumber) == 0)
            {
                throw new InvalidOperationException($"Некоректний файл: Протокол має не коректний об'єктовий або прихований номер.");
            }
            return doc;
        }

        private BaseModel ParseXmlToJsonPrime_A(XDocument document)
        {
            var config = new BaseModel
            {
                Model = document.Root.Attribute("model")?.Value,

                Zones = ParseZone(document.Root.Element("modules")
                           .Element("FxZonesModule")),

                ObjectNumber = document.Root.Element("modules")?
                    .Elements("FxNetworkModule")
                    .Elements("i")
                    .Select(x => x.Element("object_number")?.Value)
                    .FirstOrDefault(),

                HiddenNumber = document.Root.Element("modules")?
                    .Elements("FxNetworkModule")
                    .Elements("i")
                    .Select(x => x.Element("hidden_number")?.Value)
                    .FirstOrDefault(),

                TestPeriod = document.Root.Element("modules")?
                    .Elements("FxNetworkModule")
                    .Elements("i")
                    .Select(x => x.Element("test_period")?.Value)
                    .FirstOrDefault(),
            };

            if (config.Zones == null || !config.Zones.Any())
            {
                throw new InvalidOperationException("Некоректний файл: немає активних зон.");
            }

            if (int.Parse(config.ObjectNumber) == 0 || int.Parse(config.HiddenNumber) == 0)
            {
                throw new InvalidOperationException($"Некоректний файл: Протокол має не коректний об'єктовий або прихований номер.");
            }
            return config;
        }

        private bool HasStructure(XElement element, string[] expectedElements)
        {
            if (element == null) return false;

            var elementNames = element.Elements().Select(e => e.Name.LocalName).ToList();

            return expectedElements.All(expected => elementNames.Contains(expected));
        }



        private static string ParseNetwork(XElement networkElement)
        {
            if (networkElement == null) return null;

            var network = networkElement.Element("PCN")?.Element("test_time")?.Value;


            if (string.IsNullOrWhiteSpace(network))
            {
                throw new InvalidOperationException("Некоректний файл: Поле 'TestTime' є порожнім.");
            }

            return network;
        }

        private  List<ZoneModel> ParseZone(XElement zoneElement)
        {
            if (zoneElement == null) return null;

            var zones = zoneElement?.Elements("i")
            .Select(x => new ZoneModel
            {
                Id = int.Parse(x.Attribute("id")?.Value ?? "0") + 1, 
                Name = string.IsNullOrWhiteSpace(x.Element("name")?.Value)
                    ? $"Зона №{x.Attribute("id")?.Value}"
                    : x.Element("name")?.Value,
                TypeZone = ZoneType.FireAddressZone,

            }).ToList();


            return zones;
        }

        private static ZoneType ConvertZoneType(string type)
        {
            return type switch
            {
                "manual" => ZoneType.ManualNotifier,
                "general" => ZoneType.AutomaticNotifier,
                _ => ZoneType.UniversalInput
            };
        }


    }
}
