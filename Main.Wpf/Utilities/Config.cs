using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Main.Wpf.Utilities
{
    public static class Config
    {
        private static string _file = "";

        private static string _extensionsDirectory;

        private static string _extensionDirectoryName = "";

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

        public static string ExtensionsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_extensionsDirectory))
                    return Path.Combine(
                        Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)),
                        "Extensions");

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

            private static string _clientId;

            private static string _apiClientId;

            private static string _apiClientSecret;

            public static string Domain
            {
                get => string.IsNullOrEmpty(_domain) ? "hampoelz.eu.auth0.com" : _domain;
                set
                {
                    if (!string.IsNullOrEmpty(_domain)) return;

                    if (_domain == value || string.IsNullOrEmpty(value)) return;

                    _domain = value;

                    LogFile.WriteLog("Change Auth0 Domain ...");
                }
            }

            public static string ClientId
            {
                get => string.IsNullOrEmpty(_clientId) ? "_9ZvrbGJUX4MfWdzt6F7pW2e0Z0Zc0OA" : _clientId;
                set
                {
                    if (!string.IsNullOrEmpty(_clientId)) return;

                    if (_clientId == value || string.IsNullOrEmpty(value)) return;

                    _clientId = value;

                    LogFile.WriteLog("Change Auth0 Client ID ...");
                }
            }

            public static string ApiClientId
            {
                get => string.IsNullOrEmpty(_apiClientId) ? "GTgQvzJvhsSPT0w8sirtIj69cTwfS9AW" : _apiClientId;
                set
                {
                    if (!string.IsNullOrEmpty(_apiClientId)) return;

                    if (_apiClientId == value || string.IsNullOrEmpty(value)) return;

                    _apiClientId = value;

                    LogFile.WriteLog("Change Auth0 API Client ID ...");
                }
            }

            public static string ApiClientSecret
            {
                get => string.IsNullOrEmpty(_apiClientSecret)
                    ? "J4db362UcFbgrQBaXb0doKt4MNEjyPh4W2kueckfCpEppl2zHzB8xyLu3N7REknh"
                    : _apiClientSecret;
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

                private static string _website;

                public static string Organisation
                {
                    get => string.IsNullOrEmpty(_organisation) ? "Hampis Projekte" : _organisation;
                    set
                    {
                        if (!string.IsNullOrEmpty(_organisation)) return;

                        if (_organisation == value || string.IsNullOrEmpty(value)) return;

                        _organisation = value;
                    }
                }

                public static string Website
                {
                    get => string.IsNullOrEmpty(_website) ? "https://hampoelz.net/" : _website;
                    set
                    {
                        if (!string.IsNullOrEmpty(_website)) return;

                        if (_website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out var uriResult) &&
                            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            _website = uriResult.ToString();
                    }
                }
            }

            public static class Developer
            {
                private static string _organisation;

                private static string _website;

                public static string Organisation
                {
                    get => string.IsNullOrEmpty(_organisation) ? "RH Utensils" : _organisation;
                    set
                    {
                        if (!string.IsNullOrEmpty(_organisation)) return;

                        if (_organisation == value || string.IsNullOrEmpty(value)) return;

                        _organisation = value;
                    }
                }

                public static string Website
                {
                    get => string.IsNullOrEmpty(_website) ? "https://rh-utensils.hampoelz.net/" : _website;
                    set
                    {
                        if (!string.IsNullOrEmpty(_website)) return;

                        if (_website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out var uriResult) &&
                            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            _website = uriResult.ToString();
                    }
                }
            }

            public static class Extension
            {
                private static int _windowHeight = 700;

                private static int _windowWidth = 1200;

                private static string _name;

                private static string _color = "blue";

                private static string _theme = "dark";

                private static string _favicon = "";

                private static string _sourceCode;

                private static string _website;

                private static string _issueTracker;

                public static int WindowHeight
                {
                    get => _windowHeight;
                    set
                    {
                        if (_windowHeight == value || value < 700) return;

                        _windowHeight = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinHeight = value;
                        mw.Height = value;

                        mw.CenterWindowOnScreen();
                    }
                }

                public static int WindowWidth
                {
                    get => _windowWidth;
                    set
                    {
                        if (_windowWidth == value || value < 1200) return;

                        _windowWidth = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinWidth = value;
                        mw.Width = value;

                        mw.CenterWindowOnScreen();
                    }
                }

                public static string Name
                {
                    get => string.IsNullOrEmpty(_name) ? "RH Utensils" : _name;
                    set
                    {
                        if (!string.IsNullOrEmpty(_name)) return;

                        if (_name == value || string.IsNullOrEmpty(value)) return;

                        _name = value;
                    }
                }

                [Obsolete]
                public static string Color
                {
                    get => _color;
                    set
                    {
                        value = value.ToLower();

                        if (_color == value || string.IsNullOrEmpty(value)) return;

                        var colors = new List<string>
                        {
                            "yellow", "amber", "deeporange", "lightblue", "teal", "cyan", "pink", "green", "deeppurple",
                            "indigo", "lightgreen", "blue", "lime", "red", "orange", "purple"
                        };

                        if (!colors.Contains(value)) return;

                        _color = value;

                        try
                        {
                            var color = new SwatchesProvider().Swatches.FirstOrDefault(a => a.Name == value);
                            if (color != null)
                            {
                                new PaletteHelper().ReplacePrimaryColor(color);
                                new PaletteHelper().ReplaceAccentColor(color);
                            }

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

                [Obsolete]
                public static string Theme
                {
                    get => _theme;
                    set
                    {
                        value = value.ToLower();

                        if (_theme == value || string.IsNullOrEmpty(value)) return;

                        var themes = new List<string> {"dark", "light"};

                        if (!themes.Contains(value)) return;

                        _theme = value;

                        LogFile.WriteLog("Update app theme ...");

                        try
                        {
                            if (value == "light")
                            {
                                new PaletteHelper().SetLightDark(false);

                                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"),
                                    ThemeManager.GetAppTheme("BaseLight"));
                            }
                            else
                            {
                                new PaletteHelper().SetLightDark(true);

                                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"),
                                    ThemeManager.GetAppTheme("BaseDark"));
                            }

                            MessageHelper.SendDataBroadcastMessage("set Theme \"" + value + "\"");
                        }
                        catch (Exception ex)
                        {
                            LogFile.WriteLog(ex);
                        }
                    }
                }

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
                                var iconUri = new Uri(_favicon, UriKind.Relative);
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

                public static string SourceCode
                {
                    get => string.IsNullOrEmpty(_sourceCode) ? "https://github.com/rh-utensils/main" : _sourceCode;
                    set
                    {
                        if (!string.IsNullOrEmpty(_sourceCode)) return;

                        if (_sourceCode == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out var uriResult) &&
                            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            _sourceCode = uriResult.ToString();
                    }
                }

                public static string Website
                {
                    get => string.IsNullOrEmpty(_website) ? "https://github.com/rh-utensils/main" : _website;
                    set
                    {
                        if (!string.IsNullOrEmpty(_website)) return;

                        if (_website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out var uriResult) &&
                            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            _website = uriResult.ToString();
                    }
                }

                public static string IssueTracker
                {
                    get => string.IsNullOrEmpty(_issueTracker)
                        ? "https://github.com/rh-utensils/main/issues/new?assignees=&labels=bug&template=fehlerbericht.md&title="
                        : _issueTracker;
                    set
                    {
                        if (!string.IsNullOrEmpty(_issueTracker)) return;

                        if (_issueTracker == value || string.IsNullOrEmpty(value)) return;

                        if (!Uri.TryCreate(value, UriKind.Absolute, out var uriResult) ||
                            uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
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
                    if (string.IsNullOrEmpty(ExtensionDirectoryName)) return;

                    await XmlHelper.SetString(File, "config/loggedIn", value.ToString());
                }

                public static async Task<bool> Get()
                {
                    if (string.IsNullOrEmpty(ExtensionDirectoryName)) return false;

                    return await XmlHelper.ReadBool(File, "loggedIn");
                }
            }

            public static class FirstRun
            {
                public static async Task Set(bool value)
                {
                    if (string.IsNullOrEmpty(ExtensionDirectoryName)) return;

                    await XmlHelper.SetString(File, "config/firstRun", value.ToString());
                }

                public static async Task<bool> Get()
                {
                    if (string.IsNullOrEmpty(ExtensionDirectoryName)) return false;

                    return await XmlHelper.ReadBool(File, "firstRun");
                }
            }
        }

        public static class Menu
        {
            private static bool _isIndexLoading;

            public static bool ChangeingSites;
            public static MenuState DefaultMenuState { get; set; }

            public static bool IsIndexLoading
            {
                get => _isIndexLoading;
                set
                {
                    if (_isIndexLoading == value) return;

                    _isIndexLoading = value;

                    Pages.Menu.ListViewMenu.IsEnabled = !value;
                }
            }

            public static List<MenuItem> Sites => (List<MenuItem>) Pages.Menu.ListViewMenu.ItemsSource;

            public static (bool HideMenu, string Path, string StartArguments) SingleSite { get; set; } =
                (false, "", "");

            public static async Task SetSites(List<MenuItem> value)
            {
                while (!Pages.Menu._registered) await Task.Delay(100);

                ChangeingSites = true;

                while (value[0].Space) value.RemoveAt(0);

                Pages.Menu.ListViewMenu.ItemsSource = null;
                Pages.Menu.ListViewMenu.ItemsSource = value;

                ChangeingSites = false;

                var margin = 100;

                foreach (var item in Sites)
                    if (item.Space)
                        margin += 20;
                    else
                        margin += 60;

                if (margin > 420) Informations.Extension.WindowHeight = 640 + margin - 420;
            }
        }

        public static class Settings
        {
            private static string _file = "";

            public static string File
            {
                get => _file;
                set
                {
                    if (string.IsNullOrEmpty(value))
                        value = @"C:\Users\{username}\AppData\Local\HampisProjekte\RH Utensils\{appName}\Settings.json";

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
                    var settings = "";

                    try
                    {
                        using (var fs = System.IO.File.Open(File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs))
                        {
                            while (!sr.EndOfStream)
                                settings = sr.ReadToEnd();
                        }

                        return settings;
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
                        using (var sw = new StreamWriter(File))
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

            public static string MainUpdateChannel
            {
                get
                {
                    try
                    {
                        var json = "";
                        var path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? throw new InvalidOperationException(), "settings.json");

                        if (!System.IO.File.Exists(path))
                        {
                            json = "{\"updateChannel\": \"release\"}";
                        }
                        else
                        {
                            using (var fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (var sr = new StreamReader(fs))
                            {
                                while (!sr.EndOfStream)
                                    json = sr.ReadToEnd();
                            }

                            if (string.IsNullOrEmpty(json)) json = "{\"updateChannel\": \"release\"}";
                        }

                        return JsonHelper.ReadString(json, "updateChannel").ToLower();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteLog("Error getting Main update channel!");
                        LogFile.WriteLog(ex);

                        return "release";
                    }
                }
                set
                {
                    try
                    {
                        var json = "";
                        var path = Path.Combine(
                            Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                            throw new InvalidOperationException(), "settings.json");

                        if (!System.IO.File.Exists(path))
                        {
                            json = "{\"updateChannel\": \"release\"}";
                        }
                        else
                        {
                            using (var fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (var sr = new StreamReader(fs))
                            {
                                while (!sr.EndOfStream)
                                    json = sr.ReadToEnd();
                            }

                            if (string.IsNullOrEmpty(json)) json = "{\"updateChannel\": \"release\"}";
                        }

                        using (var sw = new StreamWriter(path))
                        {
                            sw.Write(JsonHelper.ChangeValue(json, "updateChannel", value));
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
                private static string _newestVersion = "-";

                public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;

                public static string NewestVersion
                {
                    get => _newestVersion;
                    set
                    {
                        if (_newestVersion == value) return;

                        _newestVersion = string.IsNullOrEmpty(value) ? "-" : value;
                    }
                }

                public static string VersionsHistoryFile =>
                    "https://raw.githubusercontent.com/rh-utensils/main/master/Main.Wpf/VersionHistory.xml";
            }

            public static class Extension
            {
                private static string _newestVersion = "-";

                private static string _versionsHistoryFile = "";
                public static Version Version { get; set; }

                public static Version RunningVersion { get; set; }

                public static string NewestVersion
                {
                    get => _newestVersion;
                    set
                    {
                        if (_newestVersion == value) return;

                        _newestVersion = string.IsNullOrEmpty(value) ? "-" : value;
                    }
                }

                public static string VersionsHistoryFile
                {
                    get => _versionsHistoryFile;
                    set
                    {
                        if (_versionsHistoryFile == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out var uriResult) &&
                            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            _versionsHistoryFile = uriResult.ToString();
                    }
                }
            }
        }
    }
}