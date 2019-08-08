using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace Main.Wpf.Utilities
{
    public static class SettingsHelper
    {
        private static bool _sync;

        private static FileSystemWatcher _settingsWatcher;

        private static bool _syncInUse;

        private static bool _syncSettingsOnChange;

        private static bool _syncWithServer;

        private static async Task WaitforSync()
        {
            while (_syncWithServer) await Task.Delay(100);
        }

        public static async Task StartSync()
        {
            if (string.IsNullOrEmpty(Config.ExtensionDirectoryName)) return;

            if (!await Config.Login.LoggedIn.Get()) return;
            {
                LogFile.WriteLog("Enable account sync ...");

                await Task.Run(SyncWithServer);

                await WaitforSync();

                var syncTimer = new Timer();
                syncTimer.Elapsed += SyncTimer_Tick;
                syncTimer.Interval = 60000;
                syncTimer.Start();

                _syncSettingsOnChange = true;
            }

            _sync = true;
        }

        private static async void SyncTimer_Tick(object sender, EventArgs e)
        {
            await Task.Run(SyncWithServer);
        }

        public static void CreateFile()
        {
            try
            {
                var defaultSettingsFile = "{" +
                                          "\"lastChange\": \"01.01.0001 00:00:00\"," +
                                          "\"updateChannel\": \"release\"," +
                                          "\"menuState\": \"expanded\"," +
                                          "\"theme\": \"" + XmlHelper.ReadString(Config.File, "defaultTheme").Result +
                                          "\""
                                          + "}";

                if (!File.Exists(Config.Settings.File))
                {
                    LogFile.WriteLog("Create Local Settings File ...");
                    Directory.CreateDirectory(Path.GetDirectoryName(Config.Settings.File) ??
                                              throw new InvalidOperationException());
                    Config.Settings.Json = defaultSettingsFile;
                }

                if (Config.Settings.Json?.Length != 0) return;
                LogFile.WriteLog("Local Settings File is empty - load defaults ...");
                Config.Settings.Json = defaultSettingsFile;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        [Obsolete]
        public static void CreateSettingsWatcher()
        {
            if (_settingsWatcher == null)
            {
                var path = Path.GetDirectoryName(Config.Settings.File);
                var filename = Path.GetFileName(Config.Settings.File);

                _settingsWatcher = new FileSystemWatcher();

                _settingsWatcher.Changed += OnChanged;

                _settingsWatcher.Path = path;
                _settingsWatcher.Filter = filename;

                _settingsWatcher.EnableRaisingEvents = true;
            }
            else
            {
                var path = Path.GetDirectoryName(Config.Settings.File);
                var filename = Path.GetFileName(Config.Settings.File);

                _settingsWatcher.Path = path;
                _settingsWatcher.Filter = filename;
            }
        }

        [Obsolete]
        private static async void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (_syncInUse) return;

                LogFile.WriteLog("Settings File change detected");

                _syncInUse = true;

                if (_sync)
                    Config.Settings.Json = JsonHelper.ChangeValue(Config.Settings.Json, "lastChange",
                        DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

                SendSettingsBroadcast();

                await Task.Delay(500);

                _syncInUse = false;

                if (Application.Current.Dispatcher != null)
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() =>
                            Config.Informations.Extension.Theme =
                                JsonHelper.ReadString(Config.Settings.Json, "theme")));
                if (_sync && _syncSettingsOnChange) await Task.Run(SyncWithServer);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static void SyncWithServer()
        {
            LogFile.WriteLog("Synchronize settings with the server ...");

            _syncWithServer = true;

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
                    lastServerSync = DateTime.Parse(JsonHelper.ReadString(serverJson, "lastChange"),
                        CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastServerSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                try
                {
                    lastLocalSync = DateTime.Parse(JsonHelper.ReadString(localJson, "lastChange"),
                        CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastLocalSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                if (lastServerSync == DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture) ||
                    string.IsNullOrEmpty(serverJson))
                {
                    AccountHelper.SetMetadata(JsonHelper.ChangeValue(localJson, "lastChange",
                        DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

                    goto Sync;
                }

                if (string.IsNullOrEmpty(lastLocalSync.ToString(CultureInfo.InvariantCulture)) ||
                    string.IsNullOrEmpty(localJson))
                    goto Finish;

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
            _syncWithServer = false;
        }

        public static void SendSettingsBroadcast()
        {
            foreach (var token in JObject.Parse(Config.Settings.Json))
            {
                var elements = Regex.Split(token.ToString().Substring(1, token.ToString().Length - 2), ", ");

                var parameter = elements[0];
                var value = elements[1];

                MessageHelper.SendDataBroadcastMessage("set SettingProperty \"" + parameter + "\" \"" + value + "\"");
            }
        }
    }
}