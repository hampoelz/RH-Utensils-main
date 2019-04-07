using Main.Wpf.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Main.Wpf
{
    public partial class App : Application
    {
        public static string Parameters;

        //Config
        public static string ConfigFile = @"";

        public static string Name = "RH Utensils";
        public static string Color = "Blue";
        public static string DefaultTheme = "Dark";
        public static string Favicon = "";

        //Extension
        public static string ExtensionsDirectory = Path.GetFullPath(@"..\Extensions");

        public static string ExtensionName = "";

        //Auth0 access
        public static string Auth0Domain = "hampoelz.eu.auth0.com";

        public static string Auth0ApiClientId = "GTgQvzJvhsSPT0w8sirtIj69cTwfS9AW";

        public static string Auth0ApiClientSecret = "J4db362UcFbgrQBaXb0doKt4MNEjyPh4W2kueckfCpEppl2zHzB8xyLu3N7REknh";
        public static string Auth0ClientId = "_9ZvrbGJUX4MfWdzt6F7pW2e0Z0Zc0OA";

        //Updates
        public static bool CustomVersion;

        public static Version ProgrammVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static string ProgrammVersionsHistoryFile = "https://raw.githubusercontent.com/rh-utensils/main/master/Main.Wpf/VersionHistory.xml";
        public static string ProgrammUpdateChannel;
        public static string ProgrammUpdateVersion = "-";

        public static Version ExtensionMaxVersion = new Version("0.0");
        public static Version ExtensionVersion = new Version("0.0");
        public static string ExtensionVersionsHistoryFile;
        public static string ExtensionUpdateVersion = "-";

        //Informations
        public static string Copyright = "Hampis Projekte";

        public static string CopyrightWebsite = "https://hampoelz.net/";

        public static string Developer = "RH Utensils";
        public static string DeveloperWebsite = "https://rh-utensils.hampoelz.net/";

        public static string SourceCode = "https://github.com/rh-utensils/main";
        public static string Website = "https://github.com/rh-utensils/main";

        //Sites
        public static string MenuState = "expanded";

        public static bool ShowFirstPage;
        public static bool ShowLogin;

        public static bool SkipLogin;

        public static List<string> SitesIcons = new List<string> { "", "", "" };
        public static List<string> SitesPaths = new List<string> { "selector.exe", "info.exe", "account.exe" };
        public static List<string> SitesPathsArguments = new List<string> { "", "", "" };
        public static List<string> SitesTitles = new List<string> { "Add-ons", "Information", "Dein Konto" };

        public static bool HideMenu;
        public static string Exe = "";
        public static string ExeArguments = "";
        public static int ExeLoadTime;

        //Settings
        public static string SettingsFile =
            @"C:\Users\{username}\AppData\Local\HampisProjekte\RH Utensils\{appName}\Settings.json";

        public static string SettingsJson;

        //Error
        public static string IssueMail = "12353798@fire.fundersclub.com";

        private static void ReadConfig()
        {
            try
            {
                if (Xml.ReadString(ConfigFile, "name") != "")
                    Name = Xml.ReadString(ConfigFile, "name");

                if (Xml.ReadString(ConfigFile, "settingsFile") != "")
                    SettingsFile = ReplaceVariables(Xml.ReadString(ConfigFile, "settingsFile"));

                if (Uri.TryCreate(Xml.ReadString(ConfigFile, "versionsHistoryFile"), UriKind.Absolute, out var uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    ExtensionVersionsHistoryFile = uriResult.ToString();

                if (Validation.IsImageValid(ReplaceVariables(Xml.ReadString(ConfigFile, "favicon"))))
                    Favicon = ReplaceVariables(Xml.ReadString(ConfigFile, "favicon"));

                if (Uri.TryCreate(Xml.ReadString(ConfigFile, "website"), UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    Website = uriResult.ToString();
                if (Uri.TryCreate(Xml.ReadString(ConfigFile, "sourceCode"), UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    SourceCode = uriResult.ToString();
                if (Uri.TryCreate(Xml.ReadString(ConfigFile, "developerWebsite"), UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    DeveloperWebsite = uriResult.ToString();
                if (Uri.TryCreate(Xml.ReadString(ConfigFile, "copyrightWebsite"), UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    CopyrightWebsite = uriResult.ToString();
                if (Xml.ReadString(ConfigFile, "developer") != "")
                    Developer = Xml.ReadString(ConfigFile, "developer");
                if (Xml.ReadString(ConfigFile, "copyright") != "")
                    Copyright = Xml.ReadString(ConfigFile, "copyright");

                if (Validation.IsMailValid(Xml.ReadString(ConfigFile, "issueMail")))
                    IssueMail = Xml.ReadString(ConfigFile, "issueMail");

                if (Xml.ReadString(ConfigFile, "auth0Domain") != "" &&
                    Xml.ReadString(ConfigFile, "auth0ClientId") != "" &&
                    Xml.ReadString(ConfigFile, "auth0APIClientId") != "" &&
                    Xml.ReadString(ConfigFile, "auth0APIClientSecret") != "")
                {
                    Auth0Domain = Xml.ReadString(ConfigFile, "auth0Domain");
                    Auth0ClientId = Xml.ReadString(ConfigFile, "auth0ClientId");
                    Auth0ApiClientId = Xml.ReadString(ConfigFile, "auth0APIClientId");
                    Auth0ApiClientSecret = Xml.ReadString(ConfigFile, "auth0APIClientSecret");
                }

                if (Xml.ReadStringList(ConfigFile, "sitePath").Count ==
                    Xml.ReadStringList(ConfigFile, "siteTitle").Count &&
                    Xml.ReadStringList(ConfigFile, "sitePathArgument").Count ==
                    Xml.ReadStringList(ConfigFile, "siteTitle").Count &&
                    Xml.ReadStringList(ConfigFile, "siteIcon").Count ==
                    Xml.ReadStringList(ConfigFile, "siteTitle").Count)
                {
                    SitesTitles = Xml.ReadStringList(ConfigFile, "siteTitle");
                    SitesPaths = ReplaceVariables(Xml.ReadStringList(ConfigFile, "sitePath"));
                    SitesPathsArguments = ReplaceVariables(Xml.ReadStringList(ConfigFile, "sitePathArgument"));

                    SitesIcons.Clear();

                    for (var i = 0; i != Xml.ReadStringList(ConfigFile, "siteIcon").Count; ++i)
                    {
                        if (Validation.IsImageValid(ReplaceVariables(Xml.ReadStringList(ConfigFile, "siteIcon")[i])))
                            SitesIcons.Add(ReplaceVariables(Xml.ReadStringList(ConfigFile, "siteIcon")[i]));
                        else
                            SitesIcons.Add("application.png");
                    }

                    //Add space
                    SitesTitles.Add("");
                    SitesPaths.Add("");
                    SitesPathsArguments.Add("");
                    SitesIcons.Add("");

                    //Add about page
                    SitesTitles.Add("Information");
                    SitesPaths.Add("info.exe");
                    SitesPathsArguments.Add("");
                    SitesIcons.Add("");

                    //Add login/logout page
                    SitesTitles.Add("Konto");
                    SitesPaths.Add("account.exe");
                    SitesPathsArguments.Add("");
                    SitesIcons.Add("");
                }

                if (Xml.ReadString(ConfigFile, "menuState") == "expanded" ||
                    Xml.ReadString(ConfigFile, "menuState") != "collapsed")
                    MenuState = Xml.ReadString(ConfigFile, "menuState");

                if (!Xml.ReadBool(ConfigFile, "hideMenu"))
                {
                    HideMenu = Xml.ReadBool(ConfigFile, "hideMenu");

                    Exe = ReplaceVariables(Xml.ReadString(ConfigFile, "loadExe"));

                    ExeLoadTime = Validation.IsStringValidInt(Xml.ReadString(ConfigFile, "exeLoadTime"))
                        ? int.Parse(Xml.ReadString(ConfigFile, "exeLoadTime"))
                        : 500;

                    ExeArguments = Xml.ReadString(ConfigFile, "exeArguments");
                }

                if (Xml.ReadString(ConfigFile, "color") != "")
                    Color = Xml.ReadString(ConfigFile, "color");

                if (Xml.ReadString(ConfigFile, "defaultTheme") != "")
                    DefaultTheme = Xml.ReadString(ConfigFile, "defaultTheme");

                if (Xml.ReadBool(ConfigFile, "showFirstPage"))
                    ShowFirstPage = Xml.ReadBool(ConfigFile, "showFirstPage");
                else if (Xml.ReadBool(ConfigFile, "showLogin"))
                    ShowLogin = Xml.ReadBool(ConfigFile, "showLogin");
                else if (Xml.ReadBool(ConfigFile, "skipLogin"))
                    SkipLogin = Xml.ReadBool(ConfigFile, "skipLogin");
            }
            catch (Exception e)
            {
                Index.SetError(e.ToString(), "Fehler beim laden der Konfigurationsdateien");
            }
        }

        private static string ReplaceVariables(string value)
        {
            return value.Replace("{extensionsDirectory}", ExtensionsDirectory)
                .Replace("{extensionName}", ExtensionName)
                .Replace("{extensionVersion}", ExtensionVersion.ToString())
                .Replace("{appName}", Name)
                .Replace("{username}", Environment.UserName);
        }

        private static List<string> ReplaceVariables(List<string> values)
        {
            var list = new List<string>();

            for (var i = 0; i != values.Count; ++i)
                list.Add(ReplaceVariables(values[i]));

            return list;
        }

        private static List<int> StringListToIntList(List<string> stringList, int defaultInt)
        {
            var intList = new List<int>();

            for (var i = 0; i != stringList.Count; ++i)
            {
                if (Validation.IsStringValidInt(stringList[i]))
                    intList.Add(int.Parse(stringList[i]));
                else
                    intList.Add(defaultInt);
            }

            return intList;
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            Parameters = String.Join(" ", e.Args);

            try
            {
                var tempMaxVersion = new Version("1.0");

                for (var arg = 0; arg != e.Args.Length; ++arg)
                {
                    if (e.Args[arg] == "-extensionsDirectory")
                        ExtensionsDirectory = e.Args[arg + 1];

                    if (e.Args[arg] != "-version") continue;

                    try
                    {
                        tempMaxVersion = new Version(e.Args[arg + 1]);
                        CustomVersion = true;
                    }
                    catch
                    {
                        CustomVersion = false;
                    }
                }

                for (var arg = 0; arg != e.Args.Length; ++arg)
                {
                    if (!Directory.Exists(ExtensionsDirectory))
                        try
                        {
                            Directory.CreateDirectory(ExtensionsDirectory);
                        }
                        catch
                        {
                            ExtensionsDirectory = Path.GetFullPath(@"..\Extensions");
                            Directory.CreateDirectory(ExtensionsDirectory);
                        }

                    var extensionsDirectories = Directory.GetDirectories(ExtensionsDirectory);
                    var extensions = Directory.GetDirectories(ExtensionsDirectory);

                    for (var extension = 0; extensionsDirectories.Length != extension; ++extension)
                    {
                        extensions[extension] = extensions[extension].Replace(ExtensionsDirectory, "")
                            .Replace(@"\", "")
                            .Replace("/", "");

                        var installedVersions = Directory.GetDirectories(extensionsDirectories[extension]);

                        for (var iii = 0; iii != installedVersions.Length; ++iii)
                        {
                            installedVersions[iii] = installedVersions[iii]
                                .Replace(extensionsDirectories[extension], "")
                                .Replace(@"\", "")
                                .Replace("/", "");

                            try
                            {
                                new Version(installedVersions[iii]);
                            }
                            catch
                            {
                                installedVersions = installedVersions.Where(val => val != installedVersions[iii])
                                    .ToArray();
                                --iii;
                            }
                        }

                        if (!CustomVersion)
                            tempMaxVersion = installedVersions.Select(s => new Version(s)).Max();

                        if (tempMaxVersion == null)
                            continue;

                        var tempProgramDirectory = Path.Combine(extensionsDirectories[extension], tempMaxVersion.ToString());

                        var tempConfigFile = Path.Combine(tempProgramDirectory, "config.xml");

                        if (!Validation.IsXmlValid(tempConfigFile))
                            continue;

                        for (var fileAssociation = 0;
                            fileAssociation != Xml.ReadStringList(tempConfigFile, "fileAssociation").Count;
                            ++fileAssociation)
                            if (e.Args[arg]
                                .EndsWith("." + Xml.ReadStringList(tempConfigFile, "fileAssociation")[fileAssociation]))
                            {
                                ConfigFile = tempConfigFile;
                                ExtensionVersion = tempMaxVersion;
                                ExtensionMaxVersion = installedVersions.Select(s => new Version(s)).Max();
                                ExtensionName = extensions[extension];
                                ReadConfig();

                                for (var sitePathArgument = 0;
                                    sitePathArgument != SitesPathsArguments.Count;
                                    ++sitePathArgument)
                                    SitesPathsArguments[sitePathArgument] = SitesPathsArguments[sitePathArgument]
                                        .Replace("{fileAssociation}", e.Args[arg]);

                                goto ReadArguments;
                            }

                        if (e.Args[arg] != "-" + extensions[extension]) continue;

                        ConfigFile = tempConfigFile;
                        ExtensionVersion = tempMaxVersion;
                        ExtensionMaxVersion = installedVersions.Select(s => new Version(s)).Max();
                        ExtensionName = extensions[extension];
                        ReadConfig();
                    }

                    for (var sitePathArgument = 0; sitePathArgument != SitesPathsArguments.Count; ++sitePathArgument)
                        SitesPathsArguments[sitePathArgument] =
                            SitesPathsArguments[sitePathArgument].Replace("{fileAssociation}", "");

                    if (e.Args[arg] == "-config" && File.Exists(e.Args[arg + 1]))
                    {
                        ConfigFile = e.Args[arg + 1];
                        ReadConfig();
                    }

                ReadArguments:
                    switch (e.Args[arg])
                    {
                        case "-programmUpdateChannel":
                            ProgrammUpdateChannel = e.Args[arg + 1];
                            break;

                        case "-firstPage":
                            ShowFirstPage = true;
                            break;

                        case "-login":
                            ShowLogin = true;
                            break;

                        case "-skipLogin":
                            SkipLogin = true;
                            break;
                    }
                }

                if (App.ExtensionName != "")
                {
                    SettingsFile = ReplaceVariables(SettingsFile);

                    string defaultSettingsFile = "<settings>" +
                                                       "<lastChange>01.01.0001 00:00:00</lastChange>" +
                                                       "<updateChannel>release</updateChannel>" +
                                                       "<menuState>expanded</menuState>" +
                                                       "<theme>" + DefaultTheme.ToLower() + "</theme>" +
                                                       "</settings>";

                    if (!File.Exists(SettingsFile))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile));
                        File.Create(SettingsFile);
                        Functions.Settings.Set(Json.ConvertToString(Json.ConvertFromXml(defaultSettingsFile), "settings"));
                    }

                readSettings:
                    using (var fs = File.Open(SettingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs))
                    {
                        var settingsFileStream = "";
                        while (!sr.EndOfStream)
                            settingsFileStream = sr.ReadToEnd();

                        App.SettingsJson = settingsFileStream;

                        if (settingsFileStream == "")
                        {
                            Functions.Settings.Set(Json.ConvertToString(Json.ConvertFromXml(defaultSettingsFile), "settings"));
                            goto readSettings;
                        }
                    }
                }

                if (Directory.Exists(@".\update"))
                {
                    using (var batFile = new StreamWriter(File.Create(@".\update.bat")))
                    {
                        batFile.WriteLine("@echo off");
                        batFile.WriteLine("timeout /t 1 /nobreak > nul");
                        batFile.WriteLine("copy /v /y /z *.log update\\*.log");
                        batFile.WriteLine("for %%F in (*) do if not \"%%F\"==\"update.bat\" del \"%%F\"");
                        batFile.WriteLine("for /d %%D in (*) do if /i not \"%%D\"==\"update\" rd /s /q \"%%D\"");
                        batFile.WriteLine("copy /v /y /z update\\*");
                        batFile.WriteLine("rd /s /q update");
                        batFile.WriteLine("start /d \"\" \"" + AppDomain.CurrentDomain.BaseDirectory + "\" \"RH Utensils.exe\" -config \"" + App.ConfigFile + "\"");
                        batFile.WriteLine("(goto) 2>nul & del \"%~f0\"");
                    }

                    var startInfo = new ProcessStartInfo(@".\update.bat")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = Path.GetDirectoryName(@".\")
                    };
                    Process.Start(startInfo);

                    Environment.Exit(0);
                }
            }
            catch (Exception exception)
            {
                Index.SetError(exception.ToString(), "Fehler beim laden der Konfigurationsdateien");
            }

            Window window = new MainWindow();
            window.Show();

            await Task.Run(() => Updater.Update(false));
            if (App.ExtensionName != "") await Task.Run(() => Updater.Update(true));
        }
    }
}