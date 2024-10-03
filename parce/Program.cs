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

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding win1251 = Encoding.GetEncoding("windows-1251");

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


                XDocument xmlDoc = XDocument.Load(filePath);

                // Парсимо необхідні дані з блоку <zones>
                var zones = from zone in xmlDoc.Descendants("zones").Elements("i")
                            let usingValue = (string)zone.Element("using")
                            where usingValue == "on" 
                            select new
                            {
                                Name = (string)zone.Element("name"),
                                Type = (string)zone.Element("type")
                            };

                var users = from user in xmlDoc.Descendants("users").Elements("i")
                            let usingValue = (string)user.Element("using")
                            where usingValue == "on"
                            select new
                            {
                                Name = (string)user.Element("name")
                            };


                var modules = from module in xmlDoc.Descendants("modules").Elements("i")
                              let usingValue = (string)module.Element("using")
                              where usingValue == "on"
                              let typeValue = (string)module.Element("type")
                              where typeValue == "PUIZ"
                              select new
                            {
                                Name = (string)module.Element("name"),
                                Type = (string)module.Element("type")
                            };

                var networkTestTime = (string)xmlDoc.Descendants("network").Descendants("test_time")
                                                    .Where(x => (string)x.Attribute("pType") == "GPRS")
                                                    .FirstOrDefault();

                var protocols = from protocol in xmlDoc.Descendants("network").Descendants("protocols").Elements("i")
                                select new
                                {
                                    Type = (string)protocol.Element("type"),
                                    SerialKey = (string)protocol.Element("ser_key"),
                                    HiddenKey = (string)protocol.Element("hid_key")
                                };

                var result = new
                {
                    Zones = zones,
                    Users = users,
                    Network = new
                    {
                        TestTime = networkTestTime,
                        Protocols = protocols
                    },
                    Modules = modules
                };

                // Конвертуємо отримані дані в JSON
                string jsonResult = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

                // Виводимо результат в консоль
                Console.WriteLine(jsonResult);

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
