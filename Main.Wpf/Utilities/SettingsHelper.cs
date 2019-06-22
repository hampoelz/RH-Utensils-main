using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf.Utilities
{
    public static class SettingsHelper
    {
        private static async Task WaitforSync()
        {
            while (_SyncWithServer)
            {
                await Task.Delay(100);
            }

            return;
        }

        public static async Task StartSync()
        {
            if (Config.Informations.Extension.Name == "RH Utensils") return;

            if (!await Config.Login.LoggedIn.Get()) return;
            {
                LogFile.WriteLog("Enable account sync ...");

                await Task.Run(() => SyncWithServer()).ConfigureAwait(false);

                await WaitforSync();

                var syncTimer = new DispatcherTimer();
                syncTimer.Tick += SyncTimer_Tick;
                syncTimer.Interval = new TimeSpan(0, 1, 0);
                syncTimer.Start();

                _syncSettingsOnChange = true;
            }

            _Sync = true;
        }

        private static bool _Sync = false;

        public static void CreateFile()
        {
            try
            {
                var defaultSettingsFile = "{" +
                                              "\"lastChange\": \"01.01.0001 00:00:00\"," +
                                              "\"updateChannel\": \"release\"," +
                                              "\"menuState\": \"expanded\"," +
                                              "\"theme\": \"" + XmlHelper.ReadString(Config.File, "defaultTheme").Result + "\""
                                          + "}";

                if (!File.Exists(Config.Settings.File))
                {
                    LogFile.WriteLog("Create Local Settings File ...");
                    Directory.CreateDirectory(Path.GetDirectoryName(Config.Settings.File));
                    Config.Settings.Json = defaultSettingsFile;
                }

                if (Config.Settings.Json?.Length == 0)
                {
                    LogFile.WriteLog("Local Settings File is empty - load defaults ...");
                    Config.Settings.Json = defaultSettingsFile;
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static async void SyncTimer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() => SyncWithServer()).ConfigureAwait(false);
        }

        private static FileSystemWatcher SettingsWatcher;

        public static void CreateSettingsWatcher()
        {
            if (SettingsWatcher == null)
            {
                var path = Path.GetDirectoryName(Config.Settings.File);
                var filename = Path.GetFileName(Config.Settings.File);

                SettingsWatcher = new FileSystemWatcher();

                SettingsWatcher.Changed += OnChanged;

                SettingsWatcher.Path = path;
                SettingsWatcher.Filter = filename;

                SettingsWatcher.EnableRaisingEvents = true;
            }
            else
            {
                var path = Path.GetDirectoryName(Config.Settings.File);
                var filename = Path.GetFileName(Config.Settings.File);

                SettingsWatcher.Path = path;
                SettingsWatcher.Filter = filename;
            }
        }

        private static bool _syncInUse;

        private static bool _syncSettingsOnChange;

        private static async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (_syncInUse) return;

            LogFile.WriteLog("Settings File change detected");

            _syncInUse = true;

            SendSettingsBroadcast();

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Config.Informations.Extension.Theme = JsonHelper.ReadString(Config.Settings.Json, "theme")));

            if (_Sync) Config.Settings.Json = JsonHelper.ChangeValue(Config.Settings.Json, "lastChange", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

            if (_Sync && _syncSettingsOnChange) await Task.Run(() => SyncWithServer()).ConfigureAwait(false);

            await Task.Delay(500).ConfigureAwait(false);

            _syncInUse = false;
        }

        private static bool _SyncWithServer;

        private static void SyncWithServer()
        {
            LogFile.WriteLog("Synchronize settings with the server ...");

            _SyncWithServer = true;

            try
            {
            Sync:
                if (!InternetHelper.CheckConnection()) goto Finish;

                var serverJson = AccountHelper.ReadMetadata();
                var localJson = Config.Settings.Json;

                DateTime lastServerSync;
                DateTime lastLocalSync;

                try
                {
                    lastServerSync = DateTime.Parse(JsonHelper.ReadString(serverJson, "lastChange"), CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastServerSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                try
                {
                    lastLocalSync = DateTime.Parse(JsonHelper.ReadString(localJson, "lastChange"), CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastLocalSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                if (lastServerSync == DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture) || serverJson?.Length == 0)
                {
                    AccountHelper.SetMetadata(JsonHelper.ChangeValue(localJson, "lastChange", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

                    goto Sync;
                }

                if (lastLocalSync.ToString(CultureInfo.InvariantCulture)?.Length == 0 || localJson?.Length == 0) goto Finish;

                if (lastServerSync == lastLocalSync) goto Finish;
                if (lastServerSync < lastLocalSync)
                    AccountHelper.SetMetadata(localJson);
                else if (lastServerSync > lastLocalSync)
                    Config.Settings.Json = serverJson;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

        Finish:
            _SyncWithServer = false;
        }

        public static void SendSettingsBroadcast()
        {
            var settings = JObject.Parse(Config.Settings.Json);

            foreach (var token in settings)
            {
                string[] elements = Regex.Split(token.ToString().Trim('[').Trim(']'), ", ");

                var parameter = elements[0];
                var value = elements[1];

                MessageHelper.SendDataBroadcastMessage("set SettingProperty \"" + parameter + "\" \"" + value + "\"");
            }
        }
    }
}