using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace Main.Wpf.Functions
{
    public static class LogFile
    {
        public static readonly string MBaseDir = Config.ExtensionDirectoryName != "" ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, "Logs") : Path.Combine(Path.GetFullPath(@".\"), "Logs");

        public static string GetFilenameYYYMMDD(string suffix, string extension)
        {
            return DateTime.Now.ToString("yyyy_MM_dd")
                + suffix
                + extension;
        }

        public static void DeleteOldLogFiles()
        {
            foreach (var file in Directory.GetFiles(MBaseDir, "*.log"))
            {
                var fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddDays(-7))
                    fi.Delete();
            }
        }

        public static void WriteLog(string message)
        {
            try
            {
                var filename = Path.Combine(MBaseDir, GetFilenameYYYMMDD("_LOG", ".log"));
                var sw = new StreamWriter(filename, true);
                var xmlEntry = new XElement("logEntry",
                    new XElement("Date", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                    new XElement("Message", message));
                sw.WriteLine(xmlEntry);
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }

        public static void WriteLog(Exception ex)
        {
            try
            {
                var filename = Path.Combine(MBaseDir, GetFilenameYYYMMDD("_LOG", ".log"));
                var sw = new StreamWriter(filename, true);
                var xmlEntry = new XElement("logEntry",
                    new XElement("Date", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                    new XElement("Exception",
                        new XElement("Source", ex.Source),
                        new XElement("Message", ex.Message),
                        new XElement("Stack", ex.StackTrace)
                     )
                );

                if (ex.InnerException != null)
                {
                    xmlEntry.Element("Exception")
                        ?.Add(
                        new XElement("InnerException",
                            new XElement("Source", ex.InnerException.Source),
                            new XElement("Message", ex.InnerException.Message),
                            new XElement("Stack", ex.InnerException.StackTrace))
                        );
                }
                sw.WriteLine(xmlEntry);
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }
    }
}