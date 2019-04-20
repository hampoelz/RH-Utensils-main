using MahApps.Metro;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf.ExampleExtension.Functions
{
    internal static class Settings
    {
        private static string Json
        {
            get
            {
                var _Settings = "";

                using (var fs = File.Open(Config.GetSettingsFile(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                        _Settings = sr.ReadToEnd();
                }

                return _Settings;
            }
            set
            {
                if (!IsJsonValid(value)) return;

                using (var sw = new StreamWriter(Config.GetSettingsFile()))
                {
                    sw.Write(value);
                }
            }
        }

        private static FileSystemWatcher SettingsWatcher;

        public static void CreateSettingsWatcher()
        {
            if (SettingsWatcher == null)
            {
                var path = Path.GetDirectoryName(Config.GetSettingsFile());
                var filename = Path.GetFileName(Config.GetSettingsFile());

                SettingsWatcher = new FileSystemWatcher();

                SettingsWatcher.Changed += OnSettingsChange;

                SettingsWatcher.Path = path;
                SettingsWatcher.Filter = filename;

                SettingsWatcher.EnableRaisingEvents = true;
            }
            else
            {
                var path = Path.GetDirectoryName(Config.GetSettingsFile());
                var filename = Path.GetFileName(Config.GetSettingsFile());

                SettingsWatcher.Path = path;
                SettingsWatcher.Filter = filename;
            }
        }

        private static bool _isChanging;

        private static async void OnSettingsChange(object sender, FileSystemEventArgs e)
        {
            if (_isChanging) return;

            _isChanging = true;

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SetTheme(GetProperty("theme", "dark"))));

            _isChanging = false;
        }

        public static string GetProperty(string parameter, string defaultValue)
        {
            var value = GetValue(parameter);

            if (value?.Length == 0)
            {
                ChangeValue(parameter, defaultValue);
                value = GetValue(parameter);
            }

            return value;
        }

        public static void SetTheme(string theme)
        {
            if (theme == "light")
            {
                new PaletteHelper().SetLightDark(false);

                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"), ThemeManager.GetAppTheme("BaseLight"));
            }
            else
            {
                new PaletteHelper().SetLightDark(true);

                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"), ThemeManager.GetAppTheme("BaseDark"));
            }
        }

        public static void ChangeValue(string parameter, string newValue)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(Json);
            jsonObj[parameter] = newValue;

            Json = (string)JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        }

        private static string GetValue(string parameter)
        {
            try
            {
                var jsonData = JObject.Parse(Json);

                return jsonData[parameter].ToString();
            }
            catch
            {
                return "";
            }
        }

        private static bool IsJsonValid(string json)
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
    }
}