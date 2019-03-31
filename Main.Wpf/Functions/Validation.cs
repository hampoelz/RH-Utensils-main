using System;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Xml;
using System.Xml.Linq;

namespace Main.Wpf.Functions
{
    public static class Validation
    {
        public static bool IsImageValid(string file)
        {
            if (!File.Exists(file) || !file.EndsWith(".png"))
                return false;

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

        public static bool IsXmlValid(string file)
        {
            if (!File.Exists(file) || !file.EndsWith(".xml"))
                return false;

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                XDocument.Load(file);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        public static bool IsMailValid(string mail)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsStringValidInt(string value)
        {
            return int.TryParse(value, out _);
        }
    }
}