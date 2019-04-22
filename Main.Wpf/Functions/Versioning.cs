using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Main.Wpf.Functions
{
    public static class Versioning
    {
        public static string File;

        public static async Task Start()
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
                                Updater.Informations.UseCustomVersion = true;
                            }
                            catch
                            {
                                Updater.Informations.UseCustomVersion = false;
                            }
                            break;

                        case "-config":
                            Config.File = App.Parameters[arg + 1];
                            await Config.Read().ConfigureAwait(false);
                            break;

                        case "-programmUpdateChannel":
                            Properties.Settings.Default.updateChannel = App.Parameters[arg + 1];
                            Properties.Settings.Default.Save();
                            break;

                        case "-skipLogin":
                            Login.SkipLogin = true;
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

                        if (!Updater.Informations.UseCustomVersion)
                            runningVersion = installedVersions.Max(s => new Version(s));
                        else
                            runningVersion = customVersion;

                        if (runningVersion == null) continue;

                        var extensionDirectory = Path.Combine(extensionsDirectories[extension], runningVersion.ToString());

                        var configFile = Path.Combine(extensionDirectory, "config.xml");

                        if (!Validation.IsXmlValid(configFile)) continue;

                        if (FileAssociation(configFile).use)
                        {
                            File = FileAssociation(configFile).file;

                            Config.ExtensionDirectoryName = extensions[extension];

                            Updater.Informations.Extension.RunningVersion = runningVersion;
                            Updater.Informations.Extension.Version = installedVersions.Max(s => new Version(s));

                            Config.File = configFile;
                            await Config.Read().ConfigureAwait(false);

                            return;
                        }
                        else if (App.Parameters.Contains("-" + extensions[extension]))
                        {
                            Config.ExtensionDirectoryName = extensions[extension];

                            Updater.Informations.Extension.RunningVersion = runningVersion;
                            Updater.Informations.Extension.Version = installedVersions.Max(s => new Version(s));

                            Config.File = configFile;
                            await Config.Read().ConfigureAwait(false);

                            return;
                        }
                    }
                }
                else
                {
                    if (FileAssociation(Config.File).use)
                    {
                        File = FileAssociation(Config.File).file;

                        Updater.Informations.Extension.RunningVersion = customVersion;
                    }
                }

                List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>
                {
                    ("Add-ons", "", "selector.exe", ""),
                    ("Information", "", "info.exe", "")
                };
                if (!Login.SkipLogin) sites.Add(("Anmelden", "", "account.exe", ""));

                Menu.Sites = sites;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static (bool use, string file) FileAssociation(string configFile)
        {
            var fileAssociations = Xml.ReadStringList(configFile, "fileAssociation");

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