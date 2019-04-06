using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace Main.Wpf.Functions
{
    public static class Updater
    {
        public static void Update(bool updateExtension)
        {
            if (updateExtension && App.ExtensionVersionsHistoryFile == "") return;

            LogFile.WriteLog("Check for new " + (updateExtension ? "extension" : "program") + " updates ...");

            try
            {
                if (!InternetChecker.Check()) return;

                var file = updateExtension ? Path.Combine(App.ExtensionsDirectory, App.ExtensionName, App.ExtensionVersion.ToString(), "VersionHistory.xml") : Path.GetFullPath(@".\VersionHistory.xml");

                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(updateExtension ? App.ExtensionVersionsHistoryFile : App.ProgrammVersionsHistoryFile, file);
                    }
                }
                catch
                {
                    return;
                }

                var userUpdateChannel = updateExtension ? Json.ConvertToString(App.SettingsJson, "updateChannel") : Properties.Settings.Default.updateChannel;

                int tempUserUpdateChannel;

                switch (userUpdateChannel)
                {
                    case "nightly":
                        tempUserUpdateChannel = 0;
                        break;

                    case "developer":
                        tempUserUpdateChannel = 1;
                        break;

                    case "beta":
                        tempUserUpdateChannel = 2;
                        break;

                    case "release":
                        tempUserUpdateChannel = 3;
                        break;

                    default:
                        tempUserUpdateChannel = 3;
                        break;
                }

                var updateChannels = Xml.ReadStringList(file, "updateChannel");
                var updateChannel = "stable";
                int tempUpdateChannel;

                var versions = new List<Version>();

                for (var i = 0; i != updateChannels.Count; ++i)
                    versions.Add(new Version(Xml.ReadStringList(file, "version")[i]));

                var latestVersion = versions.Max();

                var serverUpdateFile = "";
                var useSetup = false;

            checkUpdateChannel:
                for (var i = 0; i != versions.Count; ++i)
                {
                    if (latestVersion != versions[i]) continue;

                    updateChannel = updateChannels[i];
                    serverUpdateFile = Xml.ReadStringList(file, "file")[i];
                    useSetup = Xml.ReadBoolList(file, "setup")[i];
                }

                switch (updateChannel)
                {
                    case "nightly":
                        tempUpdateChannel = 0;
                        break;

                    case "developer":
                        tempUpdateChannel = 1;
                        break;

                    case "beta":
                        tempUpdateChannel = 2;
                        break;

                    case "release":
                        tempUpdateChannel = 3;
                        break;

                    default:
                        tempUpdateChannel = 3;
                        break;
                }

                if (tempUpdateChannel < tempUserUpdateChannel)
                {
                    if (latestVersion == versions.Min()) goto updateFinished;

                    latestVersion = versions.Where(s => s < latestVersion).Max();
                    goto checkUpdateChannel;
                }

                var currentVersion = updateExtension ? App.ExtensionMaxVersion : App.ProgrammVersion;

                switch (updateExtension)
                {
                    case true:
                        App.ExtensionUpdateVersion = latestVersion.ToString();
                        break;

                    case false:
                        App.ProgrammUpdateVersion = latestVersion.ToString();
                        break;
                }

                if (latestVersion <= currentVersion) goto updateFinished;

                LogFile.WriteLog("New " + (updateExtension ? "extension" : "program") + " update found: Latest version: " + latestVersion + " / " + "Installed version: " + currentVersion);

                try
                {
                    if (!useSetup)
                    {
                        LogFile.WriteLog("Download and install update ...");

                        var localUpdateFile = updateExtension ? Path.Combine(App.ExtensionsDirectory, App.ExtensionName, "update.zip") : Path.GetFullPath(@".\update.zip");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(serverUpdateFile, localUpdateFile);
                        }

                        var newExtensionDirectory = updateExtension ? Path.Combine(App.ExtensionsDirectory, App.ExtensionName, latestVersion.ToString()) : Path.GetFullPath(@".\update");

                        Directory.CreateDirectory(newExtensionDirectory);
                        ZipFile.ExtractToDirectory(localUpdateFile, newExtensionDirectory);
                        File.Delete(localUpdateFile);
                    }
                    else
                    {
                        LogFile.WriteLog("Download the setup ...");

                        var localUpdateFile = updateExtension ? Path.Combine(App.ExtensionsDirectory, App.ExtensionName, "updater.exe") : Path.GetFullPath(@".\updater.exe");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(serverUpdateFile, localUpdateFile);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogFile.WriteLog(e);
                }

            updateFinished:
                File.Delete(file);
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e);
            }
        }
    }
}