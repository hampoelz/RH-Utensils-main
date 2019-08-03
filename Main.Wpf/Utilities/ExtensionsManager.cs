using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Wpf.Utilities
{
    public static class ExtensionsManager
    {
        private static string _fileToOpen = "";

        public static string FileToOpen
        {
            get => _fileToOpen;
            set
            {
                value = Path.GetFullPath(value);

                if (_fileToOpen == value || string.IsNullOrEmpty(value)) return;
                if (!File.Exists(value)) return;

                _fileToOpen = value;
            }
        }

        public static async Task LoadExtension()
        {
            try
            {
                Version customVersion = null;

                for (var arg = 0; arg != App.Parameters.Length; ++arg)
                {
                    switch (App.Parameters[arg])
                    {
                        case "-extensionsDirectory":
                            Config.ExtensionsDirectory = App.Parameters[arg + 1];
                            break;

                        case "-version":
                            try
                            {
                                customVersion = new Version(App.Parameters[arg + 1]);
                                Config.Updater.UseCustomVersion = true;
                            }
                            catch
                            {
                                Config.Updater.UseCustomVersion = false;
                            }
                            break;

                        case "-config":
                            Config.File = App.Parameters[arg + 1];
                            await ConfigHelper.Read().ConfigureAwait(false);
                            break;

                        case "-programmUpdateChannel":
                            Properties.Settings.Default.updateChannel = App.Parameters[arg + 1];
                            Properties.Settings.Default.Save();
                            break;

                        case "-skipLogin":
                            Config.Login.SkipLogin = true;
                            break;
                    }
                }

                if (Config.File?.Length == 0)
                {
                    if (!Directory.Exists(Config.ExtensionsDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(Config.ExtensionsDirectory);
                        }
                        catch
                        {
                            Config.ExtensionsDirectory = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)), "Extensions");
                            Directory.CreateDirectory(Config.ExtensionsDirectory);
                        }
                    }

                    var extensionsDirectories = Directory.GetDirectories(Config.ExtensionsDirectory);
                    var extensions = Directory.GetDirectories(Config.ExtensionsDirectory);

                    for (var extension = 0; extensionsDirectories.Length != extension; ++extension)
                    {
                        extensions[extension] = extensions[extension].Replace(Config.ExtensionsDirectory, "").Replace(@"\", "").Replace("/", "");

                        var installedVersions = Directory.GetDirectories(extensionsDirectories[extension]);

                        for (var version = 0; version != installedVersions.Length; ++version)
                        {
                            installedVersions[version] = installedVersions[version].Replace(extensionsDirectories[extension], "").Replace(@"\", "").Replace("/", "");

                            try
                            {
                                new Version(installedVersions[version]);
                            }
                            catch
                            {
                                installedVersions = installedVersions.Where(val => val != installedVersions[version]).ToArray();
                                --version;
                            }
                        }

                        Version runningVersion = null;

                        if (!Config.Updater.UseCustomVersion)
                            runningVersion = installedVersions.Max(s => new Version(s));
                        else
                            runningVersion = customVersion;

                        if (runningVersion == null) continue;

                        var extensionDirectory = Path.Combine(extensionsDirectories[extension], runningVersion.ToString());

                        var configFile = Path.Combine(extensionDirectory, "config.xml");

                        if (!ValidationHelper.IsXmlValid(configFile)) continue;

                        if (FileAssociation(configFile).use)
                        {
                            FileToOpen = FileAssociation(configFile).file;

                            Config.ExtensionDirectoryName = extensions[extension];

                            Config.Updater.Extension.RunningVersion = runningVersion;
                            Config.Updater.Extension.Version = installedVersions.Max(s => new Version(s));

                            Config.File = configFile;
                            await ConfigHelper.Read().ConfigureAwait(false);

                            return;
                        }
                        else if (App.Parameters.Contains("-" + extensions[extension]))
                        {
                            Config.ExtensionDirectoryName = extensions[extension];

                            Config.Updater.Extension.RunningVersion = runningVersion;
                            Config.Updater.Extension.Version = installedVersions.Max(s => new Version(s));

                            Config.File = configFile;
                            await ConfigHelper.Read().ConfigureAwait(false);

                            return;
                        }
                    }
                }
                else
                {
                    if (FileAssociation(Config.File).use)
                    {
                        FileToOpen = FileAssociation(Config.File).file;

                        Config.Updater.Extension.RunningVersion = customVersion;
                    }
                }

                List<MenuItem> sites = new List<MenuItem>
                {
                    new MenuItem() {Title = "Add-ons", Icon = PackIconKind.ExtensionOutline, Path = "selector.exe"},
                    new MenuItem() {Title = "Information", Icon = PackIconKind.InformationOutline, Path = "info.exe"}
                };

                if (!Config.Login.SkipLogin && Config.Informations.Extension.Name != "RH Utensils") sites.Add(new MenuItem() { Title = "Anmelden", Icon = PackIconKind.Login, Path = "account.exe" });

                await Config.Menu.SetSites(sites);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static (bool use, string file) FileAssociation(string configFile)
        {
            var fileAssociations = XmlHelper.ReadStringList(configFile, "fileAssociation");

            for (var arg = 0; arg != App.Parameters.Length; ++arg)
            {
                for (var fileAssociation = 0; fileAssociation != fileAssociations.Count; ++fileAssociation)
                {
                    if (App.Parameters[arg].EndsWith("." + fileAssociations[fileAssociation]))
                    {
                        return (true, App.Parameters[arg]);
                    }
                }
            }

            return (false, "");
        }
    }
}