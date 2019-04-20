using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf.Functions
{
    public static class Config
    {
        private static string file = "";

        public static string File
        {
            get
            {
                return file;
            }
            set
            {
                value = Path.GetFullPath(ReplaceVariables.Replace(value));

                if (file == value || value?.Length == 0) return;
                if (!System.IO.File.Exists(value)) return;
                if (!Validation.IsXmlValid(value)) return;

                file = value;

                CreateConfigWatcher();

                LogFile.WriteLog("Change Config File path ...");
            }
        }

        private static string extensionsDirectory = Path.GetFullPath(@"..\Extensions");

        public static string ExtensionsDirectory
        {
            get
            {
                return extensionsDirectory;
            }
            set
            {
                value = Path.GetFullPath(value);

                if (extensionsDirectory == value || value?.Length == 0) return;
                if (!Directory.Exists(value)) return;

                extensionsDirectory = value;

                LogFile.WriteLog("Change Extensions Directory path ...");
            }
        }

        private static string extensionDirectoryName = "";

        public static string ExtensionDirectoryName
        {
            get
            {
                return extensionDirectoryName;
            }
            set
            {
                if (extensionDirectoryName == value || value?.Length == 0) return;

                extensionDirectoryName = value;

                LogFile.WriteLog("Set Extension Directory Name ...");
            }
        }

        public static async Task Read()
        {
            var file = File;

            LogFile.WriteLog("Read Config File ...");

            try
            {
                Informations.Extension.Name = await Xml.ReadString(file, "name").ConfigureAwait(false);

                Settings.File = await Xml.ReadString(file, "settingsFile").ConfigureAwait(false);

                Settings.File = await Xml.ReadString(file, "settingsFile").ConfigureAwait(false);
                Informations.Extension.Color = await Xml.ReadString(file, "color").ConfigureAwait(false);
                Informations.Extension.Theme = Json.ReadString(Settings.Json, "theme");
                Informations.Extension.Favicon = await Xml.ReadString(file, "favicon").ConfigureAwait(false);

                Menu.SingleSite = (await Xml.ReadBool(file, "hideMenu").ConfigureAwait(false), await Xml.ReadString(file, "singleSite_Path").ConfigureAwait(false), await Xml.ReadString(file, "singleSite_StartArguments").ConfigureAwait(false));

                List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();
                var Titels = Xml.ReadStringList(file, "site_Title");
                var Icons = Xml.ReadStringList(file, "site_Icon");
                var Paths = Xml.ReadStringList(file, "site_Path");
                var StartArguments = Xml.ReadStringList(file, "site_StartArguments");

                for (var site = 0; site != Titels.Count; ++site)
                {
                    sites.Add((Titels[site],Icons[site], Paths[site], StartArguments[site]));
                }
                sites.Add(("", "", "", ""));
                sites.Add(("Information", "", "info.exe", ""));
                if (!Login.SkipLogin) sites.Add(("Anmelden", "", "account.exe", ""));

                Menu.Sites = sites;

                Menu.DefaultMenuState = Menu.StringToMenuState(await Xml.ReadString(file, "defaultMenuState").ConfigureAwait(false));

                Login.SkipLogin = await Xml.ReadBool(file, "skipLogin").ConfigureAwait(false);

                Updater.Informations.Extension.VersionsHistoryFile = await Xml.ReadString(file, "versionsHistoryFile").ConfigureAwait(false);

                Informations.Copyright.Organisation = await Xml.ReadString(file, "copyright_Organisation").ConfigureAwait(false);
                Informations.Copyright.Website = await Xml.ReadString(file, "copyright_Website").ConfigureAwait(false);

                Informations.Developer.Organisation = await Xml.ReadString(file, "developer_Organisation").ConfigureAwait(false);
                Informations.Developer.Website = await Xml.ReadString(file, "developer_Website").ConfigureAwait(false);

                Informations.Extension.SourceCode = await Xml.ReadString(file, "extension_SourceCode").ConfigureAwait(false);
                Informations.Extension.Website = await Xml.ReadString(file, "extension_Website").ConfigureAwait(false);

                Informations.Extension.IssueTracker = await Xml.ReadString(file, "issueTracker").ConfigureAwait(false);

                Account.Auth0.Domain = await Xml.ReadString(file, "auth0_Domain").ConfigureAwait(false);
                Account.Auth0.ClientId = await Xml.ReadString(file, "auth0_ClientId").ConfigureAwait(false);
                Account.Auth0.ApiClientId = await Xml.ReadString(file, "auth0_APIClientId").ConfigureAwait(false);
                Account.Auth0.ApiClientSecret = await Xml.ReadString(file, "auth0_APIClientSecret").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogFile.WriteLog(ex);
            }
        }

        public static bool _isChanging;

        private static FileSystemWatcher ConfigWatcher;

        public static void CreateConfigWatcher()
        {
            try
            {
                if (ConfigWatcher == null)
                {
                    var path = Path.GetDirectoryName(File);
                    var filename = Path.GetFileName(File);

                    ConfigWatcher = new FileSystemWatcher();

                    ConfigWatcher.Changed += OnConfigChange;

                    ConfigWatcher.Path = path;
                    ConfigWatcher.Filter = filename;

                    ConfigWatcher.EnableRaisingEvents = true;
                }
                else
                {
                    var path = Path.GetDirectoryName(File);
                    var filename = Path.GetFileName(File);

                    ConfigWatcher.Path = path;
                    ConfigWatcher.Filter = filename;
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static async void OnConfigChange(object sender, FileSystemEventArgs e)
        {
            if (_isChanging) return;

            LogFile.WriteLog("Config File change detected");

            _isChanging = true;

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                await Read().ConfigureAwait(false);

                if (int.TryParse(await Xml.ReadString(Config.File, "selectionIndex").ConfigureAwait(false), out var index) && index - 1 >= 0)
                    await Menu.SelectMenuItemAsync(index - 1).ConfigureAwait(false);
                else
                    await Menu.SelectMenuItemAsync(0).ConfigureAwait(false);
            }));

            _isChanging = false;
        }
    }
}