using MahApps.Metro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf.Functions
{
    internal static class Settings
    {
        private static void Get()
        {
            LogFile.WriteLog("Update local settings file ...");

            var json = "";

            try
            {
                using (var fs = File.Open(App.SettingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                        json = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }

            App.SettingsJson = json;
        }

        public static void Set(string newJson)
        {
            LogFile.WriteLog("Change local settings file ...");

            try
            {
                using (var sw = new StreamWriter(App.SettingsFile))
                {
                    sw.Write(newJson);
                }

                App.SettingsJson = newJson;
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }
        }

        public static void SyncTheme()
        {
            LogFile.WriteLog("Update app theme ...");

            try
            {
                var Color = new SwatchesProvider().Swatches.FirstOrDefault(a => a.Name == App.Color.ToLower());
                new PaletteHelper().ReplacePrimaryColor(Color);
                new PaletteHelper().ReplaceAccentColor(Color);

                if (Json.ConvertToString(App.SettingsJson, "theme") == "dark")
                {
                    new PaletteHelper().SetLightDark(true);

                    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"), ThemeManager.GetAppTheme("BaseDark"));
                }
                else
                {
                    new PaletteHelper().SetLightDark(false);

                    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"), ThemeManager.GetAppTheme("BaseLight"));
                }
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }
        }

        private static bool _syncSettingsOnChange;

        public static void StartSync()
        {
            LogFile.WriteLog("Enable account sync ...");

            _syncSettingsOnChange = true;

            var syncTimer = new DispatcherTimer();
            syncTimer.Tick += SyncTimer_Tick;
            syncTimer.Interval = new TimeSpan(0, 5, 0);
            syncTimer.Start();
        }

        private static async void SyncTimer_Tick(object sender, EventArgs e)
        {
            Get();

            // ReSharper disable once ConvertClosureToMethodGroup
            await Task.Run(() => SyncWithServer());
        }

        public static void CreateFileWatcher(string file)
        {
            var path = Path.GetDirectoryName(file);
            var filename = Path.GetFileName(file);

            var watcher = new FileSystemWatcher();

            watcher.Changed += OnChanged;

            watcher.Path = path;
            watcher.Filter = filename;

            watcher.EnableRaisingEvents = true;
        }

        private static bool _syncInUse;

        private static async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (_syncInUse) return;

            LogFile.WriteLog("Settings file change detected");

            _syncInUse = true;

            Get();

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => SyncTheme()));

            Set(Json.ChangeValue(App.SettingsJson, "lastChange", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

            // ReSharper disable once ConvertClosureToMethodGroup
            if (_syncSettingsOnChange) await Task.Run(() => SyncWithServer());

            await Task.Delay(500);

            _syncInUse = false;
        }

        private static void SyncWithServer()
        {
            LogFile.WriteLog("Synchronize settings with the server ...");

            _syncInUse = true;

            try
            {
            Sync:
                if (!InternetChecker.Check())
                    return;

                var serverJson = Account.ReadMetadata();
                var localJson = App.SettingsJson;

                DateTime lastServerSync;
                DateTime lastLocalSync;

                try
                {
                    lastServerSync = DateTime.Parse(Json.ConvertToString(serverJson, "lastChange"), CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastServerSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                try
                {
                    lastLocalSync = DateTime.Parse(Json.ConvertToString(localJson, "lastChange"), CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastLocalSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                if (lastServerSync == DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture) || serverJson == "")
                {
                    Account.SetMetadata(Json.ChangeValue(localJson, "lastChange", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

                    goto Sync;
                }

                if (lastLocalSync.ToString(CultureInfo.InvariantCulture) == "" || localJson == "") return;

                if (lastServerSync == lastLocalSync)
                    return;
                if (lastServerSync < lastLocalSync)
                    Account.SetMetadata(localJson);
                else if (lastServerSync > lastLocalSync)
                    Set(serverJson);
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }

            _syncInUse = true;
        }
    }
}