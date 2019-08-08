using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Main.Wpf.Properties;

namespace Main.Wpf.Utilities
{
    public static class UpdateHelper
    {
        public enum UpdateChannels
        {
            weekly,
            developer,
            beta,
            release
        }

        public static bool IsDownloading;

        public static void Update(bool updateExtension)
        {
            if (updateExtension && string.IsNullOrEmpty(Config.ExtensionDirectoryName)) return;

            LogFile.WriteLog("Check for new " + (updateExtension ? "extension" : "program") + " updates ...");

            if (updateExtension) Config.Updater.Extension.NewestVersion = null;
            if (!updateExtension) Config.Updater.Programm.NewestVersion = null;

            try
            {
                if (!InternetHelper.CheckConnection()) return;

                var file = updateExtension
                    ? Path.Combine(Config.ExtensionsDirectory,
                        Config.ExtensionDirectoryName ?? throw new InvalidOperationException(),
                        Config.Updater.Extension.RunningVersion.ToString(), "VersionHistory.xml")
                    : Path.Combine(
                        Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                        throw new InvalidOperationException(), "VersionHistory.xml");

                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(
                            updateExtension
                                ? Config.Updater.Extension.VersionsHistoryFile
                                : Config.Updater.Programm.VersionsHistoryFile, file);
                    }
                }
                catch
                {
                    return;
                }

                var userUpdateChannel = updateExtension
                    ? (int) Enum.Parse(typeof(UpdateChannels),
                        JsonHelper.ReadString(Config.Settings.Json, "updateChannel").ToLower())
                    : (int) Enum.Parse(typeof(UpdateChannels), Settings.Default.updateChannel);

                var updateChannels = XmlHelper.ReadStringList(file, "updateChannel");

                var versions = new List<Version>();

                for (var i = 0; i != updateChannels.Count; ++i)
                    versions.Add(new Version(XmlHelper.ReadStringList(file, "version")[i]));

                var latestVersion = versions.Max();

                var serverUpdateFile = "";
                var useSetup = false;

                var updateChannel = (int) Enum.GetValues(typeof(UpdateChannels)).Cast<UpdateChannels>().Max();

                checkUpdateChannel:
                for (var i = 0; i != versions.Count; ++i)
                {
                    if (latestVersion != versions[i]) continue;

                    if (!Enum.IsDefined(typeof(UpdateChannels),
                        (int) Enum.Parse(typeof(UpdateChannels), updateChannels[i].ToLower()))) continue;

                    updateChannel = (int) Enum.Parse(typeof(UpdateChannels), updateChannels[i].ToLower());
                    serverUpdateFile = XmlHelper.ReadStringList(file, "file")[i];
                    useSetup = XmlHelper.ReadBoolList(file, "setup")[i];
                }

                if (updateChannel < userUpdateChannel)
                {
                    if (latestVersion == versions.Min()) goto updateFinished;

                    latestVersion = versions.Where(s => s < latestVersion).Max();
                    goto checkUpdateChannel;
                }

                var currentVersion =
                    updateExtension ? Config.Updater.Extension.Version : Config.Updater.Programm.Version;

                switch (updateExtension)
                {
                    case true:
                        Config.Updater.Extension.NewestVersion = latestVersion.ToString();
                        break;

                    case false:
                        Config.Updater.Programm.NewestVersion = latestVersion.ToString();
                        break;
                }

                if (latestVersion <= currentVersion) goto updateFinished;

                LogFile.WriteLog("New " + (updateExtension ? "extension" : "program") +
                                 " update found: Latest version: " + latestVersion + " / Installed version: " +
                                 currentVersion);

                IsDownloading = true;

                try
                {
                    if (!useSetup)
                    {
                        LogFile.WriteLog("Download and install update ...");

                        var localUpdateFile = updateExtension
                            ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, "update.zip")
                            : Path.Combine(
                                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                                throw new InvalidOperationException(), "update.zip");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(serverUpdateFile, localUpdateFile);
                        }

                        var newExtensionDirectory = updateExtension
                            ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName,
                                latestVersion.ToString())
                            : Path.Combine(
                                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                                throw new InvalidOperationException(), "update");

                        Directory.CreateDirectory(newExtensionDirectory);
                        ZipFile.ExtractToDirectory(localUpdateFile, newExtensionDirectory);
                        File.Delete(localUpdateFile);
                    }
                    else
                    {
                        LogFile.WriteLog("Download the setup ...");

                        var localUpdateFile = updateExtension
                            ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, "updater.exe")
                            : Path.Combine(
                                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                                throw new InvalidOperationException(), "updater.exe");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(serverUpdateFile, localUpdateFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogFile.WriteLog(ex);
                }

                updateFinished:
                File.Delete(file);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            IsDownloading = false;
        }

        public static void BackgroundProgrammUpdate()
        {
            try
            {
                if (!Directory.Exists(
                    Path.Combine(
                        Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                        throw new InvalidOperationException(),
                        "update"))) return;
                using (var batFile = new StreamWriter(File.Create(Path.Combine(
                    Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                    throw new InvalidOperationException(), "update.bat"))))
                {
                    batFile.WriteLine("@echo off");
                    batFile.WriteLine("timeout /t 1 /nobreak > nul");
                    batFile.WriteLine("copy /v /y /z *.log update\\*.log");
                    batFile.WriteLine("for %%F in (*) do if not \"%%F\"==\"update.bat\" del \"%%F\"");
                    batFile.WriteLine("for /d %%D in (*) do if /i not \"%%D\"==\"update\" rd /s /q \"%%D\"");
                    batFile.WriteLine("copy /v /y /z update\\*");
                    batFile.WriteLine("rd /s /q update");
                    batFile.WriteLine("start /d \"\" \"" + AppDomain.CurrentDomain.BaseDirectory +
                                      "\" \"RH Utensils.exe\" " + string.Join(" ", App.Parameters));
                    batFile.WriteLine("(goto) 2>nul & del \"%~f0\"");
                }

                var startInfo = new ProcessStartInfo(Path.Combine(
                    Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                    throw new InvalidOperationException(), "update.bat"))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                                       throw new InvalidOperationException()
                };
                Process.Start(startInfo);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        public static async Task SetupProgrammUpdate()
        {
            try
            {
                if (File.Exists(Path.Combine(Config.ExtensionsDirectory, Config.Informations.Extension.Name,
                    "updater.exe")))
                {
                    var ps = new ProcessStartInfo(Path.Combine(Config.ExtensionsDirectory,
                        Config.Informations.Extension.Name, "updater.exe"))
                    {
                        Arguments = "/VERYSILENT"
                    };
                    LogFile.WriteLog("Start setup ...");
                    Process.Start(ps);

                    Application.Current.Shutdown();
                }

                if (File.Exists(@".\updater.exe"))
                {
                    var ps = new ProcessStartInfo(@".\updater.exe")
                    {
                        Arguments = "/VERYSILENT"
                    };
                    LogFile.WriteLog("Start setup ...");
                    Process.Start(ps);

                    Application.Current.Shutdown();
                }

                LogFile.WriteLog("No setup found ...");

                if (!(Application.Current.MainWindow is MainWindow mw)) return;

                await mw.Login();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }
    }
}