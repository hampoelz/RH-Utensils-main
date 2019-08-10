using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Main.Wpf.Properties;
using MaterialDesignThemes.Wpf;

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

        [Obsolete]
        public static async Task LoadExtension()
        {
            try
            {
                Version customVersion = null;

                for (var arg = 0; arg != App.Parameters.Length; ++arg)
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
                            await ConfigHelper.Read();
                            break;

                        case "-programmUpdateChannel":
                            Config.Settings.MainUpdateChannel = App.Parameters[arg + 1];
                            break;

                        case "-skipLogin":
                            Config.Login.SkipLogin = true;
                            break;
                    }

                if (string.IsNullOrEmpty(Config.File))
                {
                    if (!Directory.Exists(Config.ExtensionsDirectory))
                        try
                        {
                            Directory.CreateDirectory(Config.ExtensionsDirectory);
                        }
                        catch
                        {
                            Config.ExtensionsDirectory =
                                Path.Combine(
                                    Path.GetDirectoryName(
                                        Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)) ??
                                    throw new InvalidOperationException(),
                                    "Extensions");
                            Directory.CreateDirectory(Config.ExtensionsDirectory);
                        }

                    var extensionsDirectories = Directory.GetDirectories(Config.ExtensionsDirectory);
                    var extensions = Directory.GetDirectories(Config.ExtensionsDirectory);

                    for (var extension = 0; extensionsDirectories.Length != extension; ++extension)
                    {
                        extensions[extension] = extensions[extension].Replace(Config.ExtensionsDirectory, "")
                            .Replace(@"\", "").Replace("/", "");

                        var installedVersions = Directory.GetDirectories(extensionsDirectories[extension]);

                        for (var version = 0; version != installedVersions.Length; ++version)
                        {
                            installedVersions[version] = installedVersions[version]
                                .Replace(extensionsDirectories[extension], "").Replace(@"\", "").Replace("/", "");

                            try
                            {
                                var unused = new Version(installedVersions[version]);
                            }
                            catch
                            {
                                installedVersions = installedVersions.Where(val => val != installedVersions[version])
                                    .ToArray();
                                --version;
                            }
                        }

                        var runningVersion = !Config.Updater.UseCustomVersion
                            ? installedVersions.Max(s => new Version(s))
                            : customVersion;

                        if (runningVersion == null) continue;

                        var extensionDirectory =
                            Path.Combine(extensionsDirectories[extension], runningVersion.ToString());

                        var configFile = Path.Combine(extensionDirectory, "config.xml");

                        if (!ValidationHelper.IsXmlValid(configFile)) continue;

                        if (FileAssociation(configFile).use)
                        {
                            FileToOpen = FileAssociation(configFile).file;

                            Config.ExtensionDirectoryName = extensions[extension];

                            Config.Updater.Extension.RunningVersion = runningVersion;
                            Config.Updater.Extension.Version = installedVersions.Max(s => new Version(s));

                            Config.File = configFile;
                            await ConfigHelper.Read();

                            return;
                        }

                        if (!App.Parameters.Contains("-" + extensions[extension])) continue;

                        Config.ExtensionDirectoryName = extensions[extension];

                        Config.Updater.Extension.RunningVersion = runningVersion;
                        Config.Updater.Extension.Version = installedVersions.Max(s => new Version(s));

                        Config.File = configFile;
                        await ConfigHelper.Read();

                        return;
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

                var sites = new List<MenuItem>
                {
                    new MenuItem {Title = "Add-ons", Icon = PackIconKind.ExtensionOutline, Path = "selector.exe"},
                    new MenuItem {Title = "Information", Icon = PackIconKind.InformationOutline, Path = "info.exe"}
                };

                await Config.Menu.SetSites(sites);

                ConfigHelper._loaded = true;
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
            for (var fileAssociation = 0; fileAssociation != fileAssociations.Count; ++fileAssociation)
                if (App.Parameters[arg].EndsWith("." + fileAssociations[fileAssociation]))
                    return (true, App.Parameters[arg]);

            return (false, "");
        }
    }
}