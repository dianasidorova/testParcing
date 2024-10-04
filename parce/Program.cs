using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text;
using Formatting = Newtonsoft.Json.Formatting;
using System.Reflection;
using parce;



class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML файли (*.xml)|*.xml",
                Title = "Виберіть XML файл"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                Encoding win1251 = Encoding.GetEncoding("windows-1251");

                using (StreamReader reader = new StreamReader(filePath, win1251))
                {
                    XDocument xmlDoc = XDocument.Load(filePath);
                    string originalFileName = Path.GetFileNameWithoutExtension(filePath);
                    string jsonResult = XmlToJsonParser.ParseXmlToJson(filePath);


                    Console.WriteLine(jsonResult);

                    // Save the JSON output to a file with the same name as the XML file
                    string jsonFilePath = Path.ChangeExtension(filePath, ".json");

                    //string originalFileName = Path.GetFileNameWithoutExtension(filePath);
                    //string outputFilePath = $"{originalFileName}.json";

                    using (StreamWriter writer = new StreamWriter(jsonFilePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine(jsonResult);
                    }   

                    //var zones = from zone in xmlDoc.Descendants("zones").Elements("i")
                    //            let usingValue = (string)zone.Element("using")
                    //            where usingValue == "on"
                    //            select new
                    //            {
                    //                Name = (string)zone.Element("name"),
                    //                Type = (string)zone.Element("type")
                    //            };

                    //var users = from user in xmlDoc.Descendants("users").Elements("i")
                    //            let usingValue = (string)user.Element("using")
                    //            where usingValue == "on"
                    //            select new
                    //            {
                    //                Name = (string)user.Element("name")
                    //            };


                    //var modules = from module in xmlDoc.Descendants("modules").Elements("i")
                    //              let usingValue = (string)module.Element("using")
                    //              where usingValue == "on"
                    //              let typeValue = (string)module.Element("type")
                    //              where typeValue == "PUIZ"
                    //              select new
                    //              {
                    //                  Name = (string)module.Element("name"),
                    //                  Type = (string)module.Element("type")
                    //              };

                    //var networkTestTime = (string)xmlDoc.Descendants("network").Descendants("test_time")
                    //                                    .Where(x => (string)x.Attribute("pType") == "GPRS")
                    //                                    .FirstOrDefault();

                    //var protocols = from protocol in xmlDoc.Descendants("network").Descendants("protocols").Elements("i")
                    //                select new
                    //                {
                    //                    Type = (string)protocol.Element("type"),
                    //                    SerialKey = (string)protocol.Element("ser_key"),
                    //                    HiddenKey = (string)protocol.Element("hid_key")
                    //                };

                    //var result = new Dictionary<string, object>();

                    //if (zones.Any())
                    //{
                    //    result["Zones"] = zones;
                    //}
                    //if (users.Any())
                    //{
                    //    result["Users"] = users;
                    //}


                    //var networkBlock = new Dictionary<string, object>();
                    //if (!string.IsNullOrEmpty(networkTestTime))
                    //{
                    //    networkBlock["TestTime"] = networkTestTime;
                    //}
                    //if (protocols.Any())
                    //{
                    //    networkBlock["Protocols"] = protocols;
                    //}

                    //if (networkBlock.Any())
                    //{
                    //    result["Network"] = networkBlock;
                    //}

                    //if (modules.Any())
                    //{
                    //    result["Modules"] = modules;
                    //}

                    //if (result.Any())
                    //{
                    //    string jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);

                    //    Console.OutputEncoding = Encoding.UTF8;
                    //    Console.WriteLine(jsonResult);



                    //    using (StreamWriter writer = new StreamWriter(outputFilePath, false, Encoding.UTF8))
                    //    {
                    //        writer.WriteLine(jsonResult);
                    //    }

                    Console.WriteLine($"Результат збережено у файл: {jsonFilePath}");
                    //}
                }
            }
            else
            {
                Console.WriteLine("Файл не вибрано.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Сталася помилка: " + ex.Message);
        }
    }
}
