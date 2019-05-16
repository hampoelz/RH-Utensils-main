using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Main.Wpf.Utilities
{
    public static class JsonHelper
    {
        public static string ReadString(string json, string parameter)
        {
            var value = "";

            try
            {
                var jsonData = JObject.Parse(json);

                value = jsonData[parameter].ToString();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return value;
        }

        public static bool ReadBool(string json, string parameter)
        {
            var value = false;

            try
            {
                var jsonData = JObject.Parse(json);

                value = Convert.ToBoolean(jsonData[parameter].ToString());
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return value;
        }

        public static string ChangeValue(string json, string parameter, string newValue)
        {
            var output = "";

            try
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj[parameter] = newValue;

                output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            return output;
        }
    }
}