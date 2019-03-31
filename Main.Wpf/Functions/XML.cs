using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Main.Wpf.Functions
{
    public static class Xml
    {
        public static string ReadString(string path, string parameter)
        {
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
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                Index.SetError(e.ToString(), "Fehler beim Laden einer Konfigurationsdatei", path);
            }

            return value;
        }

        public static bool ReadBool(string path, string parameter)
        {
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
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                Index.SetError(e.ToString(), "Fehler beim Laden einer Konfigurationsdatei", path);
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
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                Index.SetError(e.ToString(), "Fehler beim Laden einer Konfigurationsdatei", path);
            }

            return values;
        }

        public static bool[] ReadBoolList(string path, string parameter)
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
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                Index.SetError(e.ToString(), "Fehler beim Laden einer Konfigurationsdatei", path);
            }

            return values.Select(Convert.ToBoolean).ToArray();
        }
    }
}