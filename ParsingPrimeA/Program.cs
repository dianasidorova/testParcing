using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using Formatting = Newtonsoft.Json.Formatting;
using System.Reflection;
using ParsingPrimeA;



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

                    string jsonFilePath = Path.ChangeExtension(filePath, ".json");

                    using (StreamWriter writer = new StreamWriter(jsonFilePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine(jsonResult);
                    }


                    Console.WriteLine($"Результат збережено у файл: {jsonFilePath}");
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
