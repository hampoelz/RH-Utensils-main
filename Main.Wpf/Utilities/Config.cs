﻿using MahApps.Metro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Main.Wpf.Utilities
{
    public static class Config
    {
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }

        private static string _file = "";

        public static string File
        {
            get => _file;
            set
            {
                value = Path.GetFullPath(ReplaceVariables.Replace(value));

                if (!string.IsNullOrEmpty(_file)) return;

                if (_file == value || string.IsNullOrEmpty(value)) return;
                if (!System.IO.File.Exists(value)) return;
                if (!ValidationHelper.IsXmlValid(value)) return;

                _file = value;
            }
        }

        private static string _extensionsDirectory;

        public static string ExtensionsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_extensionsDirectory))
                {
                    return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)), "Extensions");
                }

                return _extensionsDirectory;
            }
            set
            {
                value = Path.GetFullPath(value);

                if (!string.IsNullOrEmpty(_extensionsDirectory)) return;

                if (_extensionsDirectory == value || string.IsNullOrEmpty(value)) return;
                if (!Directory.Exists(value)) return;

                _extensionsDirectory = value;

                LogFile.WriteLog("Change Extensions Directory path ...");
            }
        }

        private static string _extensionDirectoryName = "";

        public static string ExtensionDirectoryName
        {
            get => _extensionDirectoryName;
            set
            {
                if (!string.IsNullOrEmpty(_extensionDirectoryName)) return;

                if (_extensionDirectoryName == value || string.IsNullOrEmpty(value)) return;

                _extensionDirectoryName = value;
            }
        }

        public static class Auth0
        {
            private static string _domain;

            public static string Domain
            {
                get
                {
                    if (string.IsNullOrEmpty(_domain))
                        return "hampoelz.eu.auth0.com";

                    return _domain;
                }
                set
                {
                    if (!string.IsNullOrEmpty(_domain)) return;

                    if (_domain == value || string.IsNullOrEmpty(value)) return;

                    _domain = value;

                    LogFile.WriteLog("Change Auth0 Domain ...");
                }
            }

            private static string _clientId;

            public static string ClientId
            {
                get
                {
                    if (string.IsNullOrEmpty(_clientId))
                        return "_9ZvrbGJUX4MfWdzt6F7pW2e0Z0Zc0OA";

                    return _clientId;
                }
                set
                {
                    if (!string.IsNullOrEmpty(_clientId)) return;

                    if (_clientId == value || string.IsNullOrEmpty(value)) return;

                    _clientId = value;

                    LogFile.WriteLog("Change Auth0 Client ID ...");
                }
            }

            private static string _apiClientId;

            public static string ApiClientId
            {
                get
                {
                    if (string.IsNullOrEmpty(_apiClientId))
                        return "GTgQvzJvhsSPT0w8sirtIj69cTwfS9AW";

                    return _apiClientId;
                }
                set
                {
                    if (!string.IsNullOrEmpty(_apiClientId)) return;

                    if (_apiClientId == value || string.IsNullOrEmpty(value)) return;

                    _apiClientId = value;

                    LogFile.WriteLog("Change Auth0 API Client ID ...");
                }
            }

            private static string _apiClientSecret;

            public static string ApiClientSecret
            {
                get
                {
                    if (string.IsNullOrEmpty(_apiClientSecret))
                        return "J4db362UcFbgrQBaXb0doKt4MNEjyPh4W2kueckfCpEppl2zHzB8xyLu3N7REknh";

                    return _apiClientSecret;
                }
                set
                {
                    if (!string.IsNullOrEmpty(_apiClientSecret)) return;

                    if (_apiClientSecret == value || string.IsNullOrEmpty(value)) return;

                    _apiClientSecret = value;

                    LogFile.WriteLog("Change Auth0 API Client Secret ...");
                }
            }
        }

        public static class Informations
        {
            public static class Copyright
            {
                private static string _organisation;

                public static string Organisation
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_organisation))
                            return "Hampis Projekte";

                        return _organisation;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_organisation)) return;

                        if (_organisation == value || string.IsNullOrEmpty(value)) return;

                        _organisation = value;
                    }
                }

                private static string _website;

                public static string Website
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_website))
                            return "https://hampoelz.net/";

                        return _website;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_website)) return;

                        if (_website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) _website = uriResult.ToString();
                    }
                }
            }

            public static class Developer
            {
                private static string _organisation;

                public static string Organisation
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_organisation))
                            return "RH Utensils";

                        return _organisation;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_organisation)) return;

                        if (_organisation == value || string.IsNullOrEmpty(value)) return;

                        _organisation = value;
                    }
                }

                private static string website;

                public static string Website
                {
                    get
                    {
                        if (string.IsNullOrEmpty(website))
                            return "https://rh-utensils.hampoelz.net/";

                        return website;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(website)) return;

                        if (website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                    }
                }
            }

            public static class Extension
            {
                private static int _windowHeight = 700;

                public static int WindowHeight
                {
                    get => _windowHeight;
                    set
                    {
                        if (_windowHeight == value || value < 700) return;

                        _windowHeight = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinHeight = value;

                        mw.CenterWindowOnScreen();
                    }
                }

                private static int _windowWidth = 1200;

                public static int WindowWidth
                {
                    get => _windowWidth;
                    set
                    {
                        if (_windowWidth == value || value < 1200) return;

                        _windowWidth = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinWidth = value;

                        mw.CenterWindowOnScreen();
                    }
                }

                private static string _name;

                public static string Name
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_name))
                            return "RH Utensils";

                        return _name;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_name)) return;

                        if (_name == value || string.IsNullOrEmpty(value)) return;

                        _name = value;
                    }
                }

                private static string _color = "blue";

                public static string Color
                {
                    get => _color;
                    set
                    {
                        value = value.ToLower();

                        if (_color == value || string.IsNullOrEmpty(value)) return;

                        List<string> Colors = new List<string> { "yellow", "amber", "deeporange", "lightblue", "teal", "cyan", "pink", "green", "deeppurple", "indigo", "lightgreen", "blue", "lime", "red", "orange", "purple" };

                        if (!Colors.Contains(value)) return;

                        _color = value;

                        try
                        {
                            var Color = new SwatchesProvider().Swatches.FirstOrDefault(a => a.Name == value);
                            new PaletteHelper().ReplacePrimaryColor(Color);
                            new PaletteHelper().ReplaceAccentColor(Color);

                            var palette = new PaletteHelper().QueryPalette();
                            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];
                            Pages.Menu.GridCursor.Background = new SolidColorBrush(hue.Color);

                            MessageHelper.SendDataBroadcastMessage("set Color \"" + value + "\"");
                        }
                        catch (Exception ex)
                        {
                            LogFile.WriteLog(ex);
                        }
                    }
                }

                private static string _theme = "dark";

                public static string Theme
                {
                    get => _theme;
                    set
                    {
                        value = value.ToLower();

                        if (_theme == value || string.IsNullOrEmpty(value)) return;

                        List<string> Themes = new List<string> { "dark", "light" };

                        if (!Themes.Contains(value)) return;

                        _theme = value;

                        LogFile.WriteLog("Update app theme ...");

                        try
                        {
                            if (value == "light")
                            {
                                new PaletteHelper().SetLightDark(false);

                                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"), ThemeManager.GetAppTheme("BaseLight"));
                            }
                            else
                            {
                                new PaletteHelper().SetLightDark(true);

                                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"), ThemeManager.GetAppTheme("BaseDark"));
                            }

                            MessageHelper.SendDataBroadcastMessage("set Theme \"" + value + "\"");
                        }
                        catch (Exception ex)
                        {
                            LogFile.WriteLog(ex);
                        }
                    }
                }

                private static string _favicon = "";

                public static string Favicon
                {
                    get => _favicon;
                    set
                    {
                        if (string.IsNullOrEmpty(value)) return;

                        value = Path.GetFullPath(ReplaceVariables.Replace(value));

                        if (_favicon == value) return;

                        if (!ValidationHelper.IsImageValid(value)) return;

                        _favicon = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;

                        LogFile.WriteLog("Update favicon ...");

                        try
                        {
                            if (!string.IsNullOrEmpty(_favicon))
                            {
                                Uri iconUri = new Uri(_favicon, UriKind.Relative);
                                mw.Icon = new BitmapImage(iconUri);

                                //MessageHelper.SendDataBroadcastMessage("set Favicon \"" + value + "\"");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogFile.WriteLog(ex);
                        }
                    }
                }

                private static string _sourceCode;

                public static string SourceCode
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_sourceCode))
                            return "https://github.com/rh-utensils/main";

                        return _sourceCode;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_sourceCode)) return;

                        if (_sourceCode == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) _sourceCode = uriResult.ToString();
                    }
                }

                private static string _website;

                public static string Website
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_website))
                            return "https://github.com/rh-utensils/main";

                        return _website;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_website)) return;

                        if (_website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) _website = uriResult.ToString();
                    }
                }

                private static string _issueTracker;

                public static string IssueTracker
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_issueTracker))
                            return "https://github.com/rh-utensils/main/issues/new?assignees=&labels=bug&template=fehlerbericht.md&title=";

                        return _issueTracker;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(_issueTracker)) return;

                        if (_issueTracker == value || string.IsNullOrEmpty(value)) return;

                        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                            return;
                        _issueTracker = uriResult.ToString();
                    }
                }
            }
        }

        public static class Login
        {
            public static bool SkipLogin { get; set; }

            public static class LoggedIn
            {
                public static async Task Set(bool value)
                {
                    if (Informations.Extension.Name == "RH Utensils") return;

                    await XmlHelper.SetString(Config.File, "config/loggedIn", value.ToString()).ConfigureAwait(false);
                }

                public static async Task<bool> Get()
                {
                    if (Informations.Extension.Name == "RH Utensils") return false;

                    return await XmlHelper.ReadBool(Config.File, "loggedIn").ConfigureAwait(false);
                }
            }

            public static class FirstRun
            {
                public static async Task Set(bool value)
                {
                    if (Informations.Extension.Name == "RH Utensils") return;

                    await XmlHelper.SetString(Config.File, "config/firstRun", value.ToString()).ConfigureAwait(false);
                }

                public static async Task<bool> Get()
                {
                    if (Informations.Extension.Name == "RH Utensils") return false;

                    return await XmlHelper.ReadBool(Config.File, "firstRun").ConfigureAwait(false);
                }
            }
        }

        public static class Menu
        {
            public static MenuState DefaultMenuState { get; set; }

            private static bool _isIndexLoading;

            public static bool IsIndexLoading
            {
                get => _isIndexLoading;
                set
                {
                    if (_isIndexLoading == value) return;

                    _isIndexLoading = value;

                    if (value)
                        Pages.Menu.ListViewMenu.IsEnabled = false;
                    else if (!value)
                        Pages.Menu.ListViewMenu.IsEnabled = true;
                }
            }

            public static bool _changeingSites;

            public static async Task SetSites(List<MenuItem> value)
            {
                while (!Pages.Menu._registered) await Task.Delay(100);

                _changeingSites = true;

                while (value[0].Space)
                {
                    value.RemoveAt(0);
                }

                Pages.Menu.ListViewMenu.ItemsSource = null;
                Pages.Menu.ListViewMenu.ItemsSource = value;

                _changeingSites = false;

                var margin = 100;

                foreach (MenuItem item in Sites)
                {
                    if (item.Space)
                    {
                        margin += 20;
                    }
                    else
                    {
                        margin += 60;
                    }
                }

                if (margin > 420) Informations.Extension.WindowHeight = 640 + margin - 420;
            }

            public static List<MenuItem> Sites
            {
                get
                {
                    return (List<MenuItem>)Pages.Menu.ListViewMenu.ItemsSource;
                }
            }

            public static (bool HideMenu, string Path, string StartArguments) SingleSite { get; set; } = (false, "", "");
        }

        public static class Settings
        {
            private static string _file = "";

            public static string File
            {
                get => _file;
                set
                {
                    if (string.IsNullOrEmpty(value)) value = @"C:\Users\{username}\AppData\Local\HampisProjekte\RH Utensils\{appName}\Settings.json";

                    if (!string.IsNullOrEmpty(_file)) return;

                    value = Path.GetFullPath(ReplaceVariables.Replace(value));

                    if (_file == value) return;

                    _file = value;

                    if (!System.IO.File.Exists(value)) SettingsHelper.CreateFile();
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

                    if (!ValidationHelper.IsJsonValid(value))
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
        }

        public static class Updater
        {
            public static bool UseCustomVersion { get; set; }

            public static class Programm
            {
                public static Version Version
                {
                    get
                    {
                        return Assembly.GetExecutingAssembly().GetName().Version;
                    }
                }

                private static string _newestVersion = "-";

                public static string NewestVersion
                {
                    get => _newestVersion;
                    set
                    {
                        if (_newestVersion == value) return;

                        if (string.IsNullOrEmpty(value)) _newestVersion = "-";
                        else _newestVersion = value;
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
                public static Version Version { get; set; }

                public static Version RunningVersion { get; set; }

                private static string _newestVersion = "-";

                public static string NewestVersion
                {
                    get => _newestVersion;
                    set
                    {
                        if (_newestVersion == value) return;

                        if (string.IsNullOrEmpty(value)) _newestVersion = "-";
                        else _newestVersion = value;
                    }
                }

                private static string _versionsHistoryFile = "";

                public static string VersionsHistoryFile
                {
                    get => _versionsHistoryFile;
                    set
                    {
                        if (_versionsHistoryFile == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) _versionsHistoryFile = uriResult.ToString();
                    }
                }
            }
        }
    }
}