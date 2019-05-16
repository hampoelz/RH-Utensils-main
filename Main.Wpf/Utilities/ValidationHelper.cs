using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Xml;
using System.Xml.Linq;

namespace Main.Wpf.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsImageValid(string file)
        {
            if (!File.Exists(file) || !file.EndsWith(".png")) return false;

            try
            {
                Image.FromFile(file);
                return true;
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        public static bool IsMailValid(string mail)
        {
            try
            {
                new MailAddress(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsXmlValid(string file)
        {
            if (!File.Exists(file) || !file.EndsWith(".xml")) return false;

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

        public static bool IsJsonValid(string json)
        {
            try
            {
                var obj = JObject.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsStringValidBool(string value)
        {
            return bool.TryParse(value, out _);
        }

        public static bool IsStringValidInt(string value)
        {
            return int.TryParse(value, out _);
        }
    }
}