using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Main.Wpf.Functions
{
    public static class Updater
    {
        public enum UpdateChannels { weekly, developer, beta, release };

        public static class Informations
        {
            private static bool useCustomVersion;

            public static bool UseCustomVersion
            {
                get => useCustomVersion;
                set
                {
                    if (useCustomVersion == value) return;

                    useCustomVersion = value;
                }
            }

            public static class Programm
            {
                public static Version Version
                {
                    get
                    {
                        return Assembly.GetExecutingAssembly().GetName().Version;
                    }
                }

                private static string newestVersion = "-";

                public static string NewestVersion
                {
                    get => newestVersion;
                    set
                    {
                        if (newestVersion == value) return;

                        if (value?.Length == 0 || value == null) newestVersion = "-";
                        else newestVersion = value;
                    }
                }

                public static string VersionsHistoryFile
                {
                    get
                    {
                        return "https://raw.githubusercontent.com/rh-utensils/main/master/Main.Wpf/VersionHistory.xml";
                    }
                }
            }

            public static class Extension
            {
                private static Version version;

                public static Version Version
                {
                    get => version;
                    set
                    {
                        if (version == value) return;

                        version = value;
                    }
                }

                private static Version runningVersion = null;

                public static Version RunningVersion
                {
                    get => runningVersion;
                    set
                    {
                        if (runningVersion == value) return;

                        runningVersion = value;
                    }
                }

                private static string newestVersion = "-";

                public static string NewestVersion
                {
                    get => newestVersion;
                    set
                    {
                        if (newestVersion == value) return;

                        if (value?.Length == 0 || value == null) newestVersion = "-";
                        else newestVersion = value;
                    }
                }

                private static string versionsHistoryFile = "";

                public static string VersionsHistoryFile
                {
                    get => versionsHistoryFile;
                    set
                    {
                        if (versionsHistoryFile == value || value?.Length == 0) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            versionsHistoryFile = uriResult.ToString();
                    }
                }
            }
        }

        public static void Update(bool updateExtension)
        {
            if ((updateExtension && Informations.Extension.VersionsHistoryFile?.Length == 0) || (updateExtension && Config.ExtensionDirectoryName?.Length == 0)) return;

            LogFile.WriteLog("Check for new " + (updateExtension ? "extension" : "program") + " updates ...");

            if (updateExtension) Informations.Extension.NewestVersion = null;
            if (!updateExtension) Informations.Programm.NewestVersion = null;

            try
            {
                if (!InternetChecker.Check()) return;

                var file = updateExtension ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, Informations.Extension.RunningVersion.ToString(), "VersionHistory.xml") : Path.GetFullPath(@".\VersionHistory.xml");

                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(updateExtension ? Informations.Extension.VersionsHistoryFile : Informations.Programm.VersionsHistoryFile, file);
                    }
                }
                catch
                {
                    return;
                }

                var userUpdateChannel = updateExtension ? (int)Enum.Parse(typeof(UpdateChannels), Json.ReadString(Settings.Json, "updateChannel").ToLower()) : (int)Enum.Parse(typeof(UpdateChannels), Properties.Settings.Default.updateChannel);

                var updateChannels = Xml.ReadStringList(file, "updateChannel");

                var versions = new List<Version>();

                for (var i = 0; i != updateChannels.Count; ++i)
                    versions.Add(new Version(Xml.ReadStringList(file, "version")[i]));

                var latestVersion = versions.Max();

                var serverUpdateFile = "";
                var useSetup = false;

                var updateChannel = (int)Enum.GetValues(typeof(UpdateChannels)).Cast<UpdateChannels>().Max();

            checkUpdateChannel:
                for (var i = 0; i != versions.Count; ++i)
                {
                    if (latestVersion != versions[i]) continue;

                    if (!Enum.IsDefined(typeof(UpdateChannels), (int)Enum.Parse(typeof(UpdateChannels), updateChannels[i].ToLower()))) continue;

                    updateChannel = (int)Enum.Parse(typeof(UpdateChannels), updateChannels[i].ToLower());
                    serverUpdateFile = Xml.ReadStringList(file, "file")[i];
                    useSetup = Xml.ReadBoolList(file, "setup")[i];
                }

                if (updateChannel < userUpdateChannel)
                {
                    if (latestVersion == versions.Min()) goto updateFinished;

                    latestVersion = versions.Where(s => s < latestVersion).Max();
                    goto checkUpdateChannel;
                }

                var currentVersion = updateExtension ? Informations.Extension.Version : Informations.Programm.Version;

                switch (updateExtension)
                {
                    case true:
                        Informations.Extension.NewestVersion = latestVersion.ToString();
                        break;

                    case false:
                        Informations.Programm.NewestVersion = latestVersion.ToString();
                        break;
                }

                if (latestVersion <= currentVersion) goto updateFinished;

                LogFile.WriteLog("New " + (updateExtension ? "extension" : "program") + " update found: Latest version: " + latestVersion + " / Installed version: " + currentVersion);

                try
                {
                    if (!useSetup)
                    {
                        LogFile.WriteLog("Download and install update ...");

                        var localUpdateFile = updateExtension ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, "update.zip") : Path.GetFullPath(@".\update.zip");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(serverUpdateFile, localUpdateFile);
                        }

                        var newExtensionDirectory = updateExtension ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, latestVersion.ToString()) : Path.GetFullPath(@".\update");

                        Directory.CreateDirectory(newExtensionDirectory);
                        ZipFile.ExtractToDirectory(localUpdateFile, newExtensionDirectory);
                        File.Delete(localUpdateFile);
                    }
                    else
                    {
                        LogFile.WriteLog("Download the setup ...");

                        var localUpdateFile = updateExtension ? Path.Combine(Config.ExtensionsDirectory, Config.ExtensionDirectoryName, "updater.exe") : Path.GetFullPath(@".\updater.exe");

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
        }

        public static void BackgroundProgrammUpdate()
        {
            try
            {
                if (Directory.Exists(Path.GetFullPath(@".\update")))
                {
                    using (var batFile = new StreamWriter(File.Create(Path.GetFullPath(@".\update.bat"))))
                    {
                        batFile.WriteLine("@echo off");
                        batFile.WriteLine("timeout /t 1 /nobreak > nul");
                        batFile.WriteLine("copy /v /y /z *.log update\\*.log");
                        batFile.WriteLine("for %%F in (*) do if not \"%%F\"==\"update.bat\" del \"%%F\"");
                        batFile.WriteLine("for /d %%D in (*) do if /i not \"%%D\"==\"update\" rd /s /q \"%%D\"");
                        batFile.WriteLine("copy /v /y /z update\\*");
                        batFile.WriteLine("rd /s /q update");
                        batFile.WriteLine("start /d \"\" \"" + AppDomain.CurrentDomain.BaseDirectory + "\" \"RH Utensils.exe\" -config \"" + Config.File + "\"");
                        batFile.WriteLine("(goto) 2>nul & del \"%~f0\"");
                    }

                    var startInfo = new ProcessStartInfo(Path.GetFullPath(@".\update.bat"))
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = Path.GetDirectoryName(@".\")
                    };
                    Process.Start(startInfo);

                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }
    }
}