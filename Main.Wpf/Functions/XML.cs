using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Main.Wpf.Functions
{
    public static class Xml
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

        public static async Task<string> ReadString(string path, string parameter)
        {
            while (IsFileLocked(path))
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

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
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }

            return values;
        }
    }
}