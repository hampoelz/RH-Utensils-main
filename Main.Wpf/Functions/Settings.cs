using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf.Functions
{
    public static class Settings
    {
        private static string file = "";

        public static string File
        {
            get => file;
            set
            {
                if (value?.Length == 0) value = @"C:\Users\{username}\AppData\Local\HampisProjekte\RH Utensils\{appName}\Settings.json";

                if (file != "") return;

                value = Path.GetFullPath(ReplaceVariables.Replace(value));

                if (file == value) return;

                file = value;
            }
        }

        public static string Json
        {
            get
            {
                var _Settings = "";

                try
                {
                    using (var fs = System.IO.File.Open(Settings.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                            _Settings = sr.ReadToEnd();
                    }

                    return _Settings;
                }
                catch (Exception ex)
                {
                    LogFile.WriteLog("Error getting Local Settings File!");
                    LogFile.WriteLog(ex);
                }

                return "";
            }
            set
            {
                LogFile.WriteLog("Change Local Settings File ...");

                if (!Validation.IsJsonValid(value))
                {
                    LogFile.WriteLog("Settings File is not valid!");
                    return;
                }

                try
                {
                    using (var sw = new StreamWriter(Settings.File))
                    {
                        sw.Write(value);
                    }
                }
                catch (Exception ex)
                {
                    LogFile.WriteLog(ex);
                }
            }
        }

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
            if (Informations.Extension.Name?.Length == 0 || Informations.Extension.Name == "RH Utensils") return;

            LogFile.WriteLog("Enable account sync ...");

            CreateFile();

            await Task.Run(() => SyncWithServer()).ConfigureAwait(false);

            await WaitforSync();

            CreateSettingsWatcher();

            var syncTimer = new DispatcherTimer();
            syncTimer.Tick += SyncTimer_Tick;
            syncTimer.Interval = new TimeSpan(0, 1, 0);
            syncTimer.Start();

            _syncSettingsOnChange = true;
        }

        private static void CreateFile()
        {
            try
            {
                var defaultSettingsFile = "{" +
                                              "\"lastChange\": \"01.01.0001 00:00:00\"," +
                                              "\"updateChannel\": \"release\"," +
                                              "\"menuState\": \"expanded\"," +
                                              "\"theme\": \"" + Xml.ReadString(Config.File, "defaultTheme").Result + "\"" +
                                          "}";

                if (!System.IO.File.Exists(File))
                {
                    LogFile.WriteLog("Create Local Settings File ...");
                    Directory.CreateDirectory(Path.GetDirectoryName(File));
                    Json = defaultSettingsFile;
                }

                if (Json?.Length == 0)
                {
                    LogFile.WriteLog("Local Settings File is empty - load defaults ...");
                    Json = defaultSettingsFile;
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

        private static void CreateSettingsWatcher()
        {
            if (SettingsWatcher == null)
            {
                var path = Path.GetDirectoryName(File);
                var filename = Path.GetFileName(File);

                SettingsWatcher = new FileSystemWatcher();

                SettingsWatcher.Changed += OnChanged;

                SettingsWatcher.Path = path;
                SettingsWatcher.Filter = filename;

                SettingsWatcher.EnableRaisingEvents = true;
            }
            else
            {
                var path = Path.GetDirectoryName(File);
                var filename = Path.GetFileName(File);

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

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Informations.Extension.Theme = Functions.Json.ReadString(Settings.Json, "theme")));

            Settings.Json = Functions.Json.ChangeValue(Settings.Json, "lastChange", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

            if (_syncSettingsOnChange) await Task.Run(() => SyncWithServer()).ConfigureAwait(false);

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
                if (!InternetChecker.Check()) goto Finish;

                var serverJson = Account.ReadMetadata();
                var localJson = Json;

                DateTime lastServerSync;
                DateTime lastLocalSync;

                try
                {
                    lastServerSync = DateTime.Parse(Functions.Json.ReadString(serverJson, "lastChange"), CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastServerSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                try
                {
                    lastLocalSync = DateTime.Parse(Functions.Json.ReadString(localJson, "lastChange"), CultureInfo.InvariantCulture);
                }
                catch
                {
                    lastLocalSync = DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture);
                }

                if (lastServerSync == DateTime.Parse("01.01.0001 00:00:00", CultureInfo.InvariantCulture) || serverJson?.Length == 0)
                {
                    Account.SetMetadata(Functions.Json.ChangeValue(localJson, "lastChange", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

                    goto Sync;
                }

                if (lastLocalSync.ToString(CultureInfo.InvariantCulture)?.Length == 0 || localJson?.Length == 0) goto Finish;

                if (lastServerSync == lastLocalSync) goto Finish;
                if (lastServerSync < lastLocalSync)
                    Account.SetMetadata(localJson);
                else if (lastServerSync > lastLocalSync)
                    Json = serverJson;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            Finish:
            _SyncWithServer = false;
        }
    }
}