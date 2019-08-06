using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Main.Wpf.Utilities
{
    public static class XmlHelper
    {
        private static bool IsFileLocked(string path)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
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

        public static async Task SetString(string path, string singleNode, string newValue)
        {
            while (IsFileLocked(path)) await Task.Delay(100).ConfigureAwait(false);

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                if (!(xmlDoc.SelectSingleNode(singleNode) is XmlElement node))
                {
                    var tokens = singleNode.Split('/');
                    var nodes = tokens.Take(tokens.Length - 1).ToArray();
                    var lastNode = tokens[tokens.Length - 1];

                    var parentNode = xmlDoc.SelectSingleNode(string.Join("/", nodes)) as XmlElement;
                    parentNode?.AppendChild(xmlDoc.CreateElement(lastNode));
                    xmlDoc.Save(path);

                    xmlDoc.Load(path);

                    node = xmlDoc.SelectSingleNode(singleNode) as XmlElement;
                }

                if (node != null) node.InnerText = newValue;

                xmlDoc.Save(path);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        public static async Task<string> ReadString(string path, string parameter)
        {
            while (IsFileLocked(path)) await Task.Delay(100).ConfigureAwait(false);

            var value = "";

            try
            {
                var reader = new XmlTextReader(path);

                while (reader.Read())
                {
                    var type = reader.NodeType;

                    if (type != XmlNodeType.Element) continue;

                    if (reader.Name != parameter) continue;

                    reader.Read();
                    value = reader.Value;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return value;
        }

        public static async Task<bool> ReadBool(string path, string parameter)
        {
            while (IsFileLocked(path)) await Task.Delay(100).ConfigureAwait(false);

            var value = false;

            try
            {
                var reader = new XmlTextReader(path);

                while (reader.Read())
                {
                    var type = reader.NodeType;

                    if (type != XmlNodeType.Element) continue;

                    if (reader.Name != parameter) continue;

                    reader.Read();
                    value = Convert.ToBoolean(reader.Value);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return value;
        }

        public static List<string> ReadStringList(string path, string parameter)
        {
            var values = new List<string>();

            try
            {
                var reader = new XmlTextReader(path);

                while (reader.Read())
                {
                    var type = reader.NodeType;

                    if (type != XmlNodeType.Element) continue;

                    if (reader.Name != parameter) continue;

                    reader.Read();
                    values.Add(reader.Value);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return values;
        }

        public static List<bool> ReadBoolList(string path, string parameter)
        {
            var values = new List<bool>();

            try
            {
                var reader = new XmlTextReader(path);

                while (reader.Read())
                {
                    var type = reader.NodeType;

                    if (type != XmlNodeType.Element) continue;

                    if (reader.Name != parameter) continue;

                    reader.Read();
                    values.Add(Convert.ToBoolean(reader.Value));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return values;
        }
    }
}