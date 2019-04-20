using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Main.Wpf.ExampleExtension.Functions
{
    internal static class Config
    {
        private static string file = "";

        public static string File
        {
            get
            {
                return file;
            }
            set
            {
                value = Path.GetFullPath(value);

                if (file == value || value?.Length == 0) return;
                if (!System.IO.File.Exists(value)) return;
                if (!IsXmlValid(value)) return;

                file = value;

                Settings.CreateSettingsWatcher();
            }
        }

        private static bool IsConfigLocked()
        {
            FileStream stream = null;

            try
            {
                stream = System.IO.File.Open(File, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }

        public static string GetSettingsFile()
        {
            var file = "";
            var name = "";

            var nameReader = new XmlTextReader(File);

            while (nameReader.Read())
            {
                var type = nameReader.NodeType;

                if (type != XmlNodeType.Element) continue;

                if (nameReader.Name != "name") continue;

                nameReader.Read();
                name = nameReader.Value;
            }

            nameReader.Close();

            var fileReader = new XmlTextReader(File);

            while (fileReader.Read())
            {
                var type = fileReader.NodeType;

                if (type != XmlNodeType.Element) continue;

                if (fileReader.Name != "settingsFile") continue;

                fileReader.Read();
                file = fileReader.Value;
            }

            fileReader.Close();

            if (file?.Length == 0) file = @"C:\Users\{username}\AppData\Local\HampisProjekte\RH Utensils\{appName}\Settings.json";

            return file.Replace("{username}", Environment.UserName).Replace("{extensionVersion}", Assembly.GetExecutingAssembly().GetName().Version.ToString()).Replace("{appName}", name);
        }

        public static async Task<string> ReadString(string parameter)
        {
            while (IsConfigLocked())
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            var value = "";

            var reader = new XmlTextReader(File);

            while (reader.Read())
            {
                var type = reader.NodeType;

                if (type != XmlNodeType.Element) continue;

                if (reader.Name != parameter) continue;

                reader.Read();
                value = reader.Value;
            }

            reader.Close();

            return value;
        }

        public static async Task SetString(string singleNode, string newValue)
        {
            while (IsConfigLocked())
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(File);

            if (!(xmlDoc.SelectSingleNode(singleNode) is XmlElement node))
            {
                var tokens = singleNode.Split('/');
                var nodes = tokens.Take(tokens.Length - 1).ToArray();
                var lastNode = tokens[tokens.Length - 1];

                var parentNode = xmlDoc.SelectSingleNode(string.Join("/", nodes)) as XmlElement;
                parentNode.AppendChild(xmlDoc.CreateElement(lastNode));
                xmlDoc.Save(File);

                xmlDoc.Load(File);

                node = xmlDoc.SelectSingleNode(singleNode) as XmlElement;
            }

            node.InnerText = newValue;

            xmlDoc.Save(File);
        }

        public static bool IsXmlValid(string file)
        {
            if (!System.IO.File.Exists(file) || !file.EndsWith(".xml")) return false;

            try
            {
                XDocument.Load(file);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }
    }
}