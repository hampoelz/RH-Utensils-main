using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;

namespace Main.Wpf.Pages
{
    public partial class Error
    {
        public static string ErrorMessage;
        public static string File = "-";
        public new static string Title;

        public Error()
        {
            InitializeComponent();
        }

        public static string GetFilenameYYYMMDD(string suffix, string extension)
        {
            return System.DateTime.Now.ToString("yyyy_MM_dd")
                + suffix
                + extension;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newLine = "%0D%0A";
            var configFileContent = "Datei ist nicht verfügbar!";
            var windowsVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "");

            if (System.IO.File.Exists(App.ConfigFile))
                configFileContent = "```xml"
                                    + newLine
                                    + string.Join(newLine, System.IO.File.ReadAllLines(App.ConfigFile)).Replace(Environment.NewLine, newLine)
                                    + newLine
                                    + "```";

            var logFile = Functions.LogFile.MBaseDir + GetFilenameYYYMMDD("_LOG", ".log");
            var logFileContent = "Datei ist nicht verfügbar!";

            if (System.IO.File.Exists(logFile))
                logFileContent = "```xml"
                                    + newLine
                                    + string.Join(newLine, System.IO.File.ReadAllLines(logFile)).Replace(Environment.NewLine, newLine)
                                    + newLine
                                    + "```";

            Process.Start("mailto:" + App.IssueMail
                                    + "?subject=" + Title
                                    + "&body="
                                    + "**Beschreibe den Fehler**" + newLine
                                    + "Eine klare und prägnante Beschreibung des Fehlers." + newLine
                                    + newLine
                                    + "**Reproduzieren**" + newLine
                                    + "Schritte zum Reproduzieren des Verhaltens:" + newLine
                                    + "1. Gehe zu '...'" + newLine
                                    + "2. Klicken Sie auf '....'" + newLine
                                    + "3. Blättern Sie nach unten zu '....'" + newLine
                                    + "4. Siehe Fehler" + newLine
                                    + newLine
                                    + "**Erwartetes Verhalten**" + newLine
                                    + "Eine klare und präzise Beschreibung dessen, was du erwartest hast." + newLine
                                    + newLine
                                    + "**Screenshots**" + newLine
                                    + "Füge ggf. Screenshots hinzu, um dein Problem zu erklären." + newLine
                                    + newLine
                                    + "**Protokolle**" + newLine
                                    + logFileContent + newLine
                                    + newLine
                                    + "**Konfigurationsdatei**" + newLine
                                    + configFileContent + newLine
                                    + newLine
                                    + "**Desktop:**" + newLine
                                    + "  - OS: " + windowsVersion + newLine
                                    + "  - Version " + App.ProgrammVersion + newLine
                                    + newLine
                                    + "**Zusätzlicher Kontext**" + newLine
                                    + "Füge hier einen anderen Kontext zum Problem hinzu.");
        }
    }
}