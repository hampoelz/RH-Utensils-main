using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Main.Wpf.Functions
{
    public static class Json
    {
        public static string ConvertToString(string json, string parameter)
        {
            try
            {
                var jsonData = JObject.Parse(json);

                return jsonData[parameter].ToString();
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                return "";
            }
        }

        public static string ConvertFromXml(string xml)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                return JsonConvert.SerializeXmlNode(xmlDoc);
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                return "";
            }
        }

        public static string ChangeValue(string json, string parameter, string newValue)
        {
            try
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj[parameter] = newValue;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

                return output;
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
                return "";
            }
        }
    }
}