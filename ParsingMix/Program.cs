using Newtonsoft.Json;
using ParsingMix;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

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

                    XmlToJsonParser parser = new XmlToJsonParser();

                    BaseModel documentModel = parser.ParseXmlFile(filePath);

                    string originalFileName = Path.GetFileNameWithoutExtension(filePath);
                    if (documentModel.Zones != null)
                    {
                        foreach (var zone in documentModel.Zones)
                        {
                            zone.TypeString = ConvertZoneTypeToString(zone.TypeZone);
                        }
                    }

                    string jsonResult = JsonConvert.SerializeObject(documentModel, Formatting.Indented);
                    Console.WriteLine(jsonResult);

                    string jsonFilePath = Path.ChangeExtension(filePath, ".json");

                    using (StreamWriter writer = new StreamWriter(jsonFilePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine(jsonResult);
                    }


                    Console.WriteLine($"Результат збережено: \n {jsonFilePath}");
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


    private static string ConvertZoneTypeToString(ZoneType zoneType)
    {
        return zoneType switch
        {
            ZoneType.FireAddressZone => "Адресна",
            ZoneType.UniversalInput => "Універсальний вхід",
            ZoneType.ManualNotifier => "Ручний сповіщувач",
            ZoneType.AutomaticNotifier => "Автоматичний сповіщувач",
            _ => "Невідомий тип"
        };
    }

}