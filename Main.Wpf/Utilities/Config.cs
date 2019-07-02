using System;
using System.Collections.Generic;
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
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }

        private static string file = "";
        public static string File
        {
            get => file;
            set
            {
                value = Path.GetFullPath(ReplaceVariables.Replace(value));

                if (!string.IsNullOrEmpty(file)) return;

                if (file == value || string.IsNullOrEmpty(value)) return;
                if (!System.IO.File.Exists(value)) return;
                if (!ValidationHelper.IsXmlValid(value)) return;

                file = value;
            }
        }

        private static string extensionsDirectory;
        public static string ExtensionsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(extensionsDirectory))
                {
                    return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)), "Extensions");
                }

                return extensionsDirectory;
            }
            set
            {
                value = Path.GetFullPath(value);

                if (!string.IsNullOrEmpty(extensionsDirectory)) return;

                if (extensionsDirectory == value || string.IsNullOrEmpty(value)) return;
                if (!Directory.Exists(value)) return;

                extensionsDirectory = value;

                LogFile.WriteLog("Change Extensions Directory path ...");
            }
        }

        private static string extensionDirectoryName = "";
        public static string ExtensionDirectoryName
        {
            get => extensionDirectoryName;
            set
            {
                if (!string.IsNullOrEmpty(extensionDirectoryName)) return;

                if (extensionDirectoryName == value || string.IsNullOrEmpty(value)) return;

                extensionDirectoryName = value;
            }
        }

        public static class Auth0
        {
            private static string domain;
            public static string Domain
            {
                get
                {
                    if (string.IsNullOrEmpty(domain))
                        return "hampoelz.eu.auth0.com";

                    return domain;
                }
                set
                {
                    if (!string.IsNullOrEmpty(domain)) return;

                    if (domain == value || string.IsNullOrEmpty(value)) return;

                    domain = value;

                    LogFile.WriteLog("Change Auth0 Domain ...");
                }
            }

            private static string clientId;
            public static string ClientId
            {
                get
                {
                    if (string.IsNullOrEmpty(clientId))
                        return "_9ZvrbGJUX4MfWdzt6F7pW2e0Z0Zc0OA";

                    return clientId;
                }
                set
                {
                    if (!string.IsNullOrEmpty(clientId)) return;

                    if (clientId == value || string.IsNullOrEmpty(value)) return;

                    clientId = value;

                    LogFile.WriteLog("Change Auth0 Client ID ...");
                }
            }

            private static string apiClientId;
            public static string ApiClientId
            {
                get
                {
                    if (string.IsNullOrEmpty(apiClientId))
                        return "GTgQvzJvhsSPT0w8sirtIj69cTwfS9AW";

                    return apiClientId;
                }
                set
                {
                    if (!string.IsNullOrEmpty(apiClientId)) return;

                    if (apiClientId == value || string.IsNullOrEmpty(value)) return;

                    apiClientId = value;

                    LogFile.WriteLog("Change Auth0 API Client ID ...");
                }
            }

            private static string apiClientSecret;
            public static string ApiClientSecret
            {
                get
                {
                    if (string.IsNullOrEmpty(apiClientSecret))
                        return "J4db362UcFbgrQBaXb0doKt4MNEjyPh4W2kueckfCpEppl2zHzB8xyLu3N7REknh";

                    return apiClientSecret;
                }
                set
                {
                    if (!string.IsNullOrEmpty(apiClientSecret)) return;

                    if (apiClientSecret == value || string.IsNullOrEmpty(value)) return;

                    apiClientSecret = value;

                    LogFile.WriteLog("Change Auth0 API Client Secret ...");
                }
            }
        }

        public static class Informations
        {
            public static class Copyright
            {
                private static string organisation;
                public static string Organisation
                {
                    get
                    {
                        if (string.IsNullOrEmpty(organisation))
                            return "Hampis Projekte";

                        return organisation;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(organisation)) return;

                        if (organisation == value || string.IsNullOrEmpty(value)) return;

                        organisation = value;
                    }
                }

                private static string website;
                public static string Website
                {
                    get
                    {
                        if (string.IsNullOrEmpty(website))
                            return "https://hampoelz.net/";

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

            public static class Developer
            {
                private static string organisation;
                public static string Organisation
                {
                    get
                    {
                        if (string.IsNullOrEmpty(organisation))
                            return "RH Utensils";

                        return organisation;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(organisation)) return;

                        if (organisation == value || string.IsNullOrEmpty(value)) return;

                        organisation = value;
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
                private static int windowHeight = 700;
                public static int WindowHeight
                {
                    get => windowHeight;
                    set
                    {
                        if (windowHeight == value || value < 700) return;

                        windowHeight = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinHeight = value;

                        mw.CenterWindowOnScreen();
                    }
                }

                private static int windowWidth = 1200;
                public static int WindowWidth
                {
                    get => windowWidth;
                    set
                    {
                        if (windowWidth == value || value < 1200) return;

                        windowWidth = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinWidth = value;

                        mw.CenterWindowOnScreen();
                    }
                }

                private static string name;
                public static string Name
                {
                    get
                    {
                        if (string.IsNullOrEmpty(name))
                            return "RH Utensils";

                        return name;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(name)) return;

                        if (name == value || string.IsNullOrEmpty(value)) return;

                        name = value;
                    }
                }

                private static string color = "blue";
                public static string Color
                {
                    get => color;
                    set
                    {
                        value = value.ToLower();

                        if (color == value || string.IsNullOrEmpty(value)) return;

                        List<string> Colors = new List<string> { "yellow", "amber", "deeporange", "lightblue", "teal", "cyan", "pink", "green", "deeppurple", "indigo", "lightgreen", "blue", "lime", "red", "orange", "purple" };

                        if (!Colors.Contains(value)) return;

                        color = value;

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

                private static string theme = "dark";
                public static string Theme
                {
                    get => theme;
                    set
                    {
                        value = value.ToLower();

                        if (theme == value || string.IsNullOrEmpty(value)) return;

                        List<string> Themes = new List<string> { "dark", "light" };

                        if (!Themes.Contains(value)) return;

                        theme = value;

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

                private static string favicon = "";
                public static string Favicon
                {
                    get => favicon;
                    set
                    {
                        if (string.IsNullOrEmpty(value)) return;

                        value = Path.GetFullPath(ReplaceVariables.Replace(value));

                        if (favicon == value) return;

                        if (!ValidationHelper.IsImageValid(value)) return;

                        favicon = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;

                        LogFile.WriteLog("Update favicon ...");

                        try
                        {
                            if (!string.IsNullOrEmpty(favicon))
                            {
                                Uri iconUri = new Uri(favicon, UriKind.Relative);
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

                private static string sourceCode;
                public static string SourceCode
                {
                    get
                    {
                        if (string.IsNullOrEmpty(sourceCode))
                            return "https://github.com/rh-utensils/main";

                        return sourceCode;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(sourceCode)) return;

                        if (sourceCode == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) sourceCode = uriResult.ToString();
                    }
                }

                private static string website;
                public static string Website
                {
                    get
                    {
                        if (string.IsNullOrEmpty(website))
                            return "https://github.com/rh-utensils/main";

                        return website;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(website)) return;

                        if (website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                    }
                }

                private static string issueTracker;
                public static string IssueTracker
                {
                    get
                    {
                        if (string.IsNullOrEmpty(issueTracker))
                            return "https://github.com/rh-utensils/main/issues/new?assignees=&labels=bug&template=fehlerbericht.md&title=";

                        return issueTracker;
                    }
                    set
                    {
                        if (!string.IsNullOrEmpty(issueTracker)) return;

                        if (issueTracker == value || string.IsNullOrEmpty(value)) return;

                        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                            return;
                        issueTracker = uriResult.ToString();
                    }
                }
            }
        }

        public static class Login
        {
            private static bool skipLogin;
            public static bool SkipLogin
            {
                get => skipLogin;
                set
                {
                    if (skipLogin == value) return;

                    skipLogin = value;
                }
            }

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
            private static MenuState defaultMenuState;
            public static MenuState DefaultMenuState
            {
                get => defaultMenuState;
                set
                {
                    if (defaultMenuState == value) return;

                    defaultMenuState = value;
                }
            }

            private static bool isIndexLoading;
            public static bool IsIndexLoading
            {
                get => isIndexLoading;
                set
                {
                    if (isIndexLoading == value) return;

                    isIndexLoading = value;

                    if (value)
                        Pages.Menu.ListViewMenu.IsEnabled = false;
                    else if (!value)
                        Pages.Menu.ListViewMenu.IsEnabled = true;
                }
            }

            private static readonly List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();
            public static List<(string Title, string Icon, string Path, string StartArguments)> Sites
            {
                get => sites;
                set
                {
                    if (sites == value) return;
                    if (singleSite.HideMenu) return;

                    int newIndex = value.Count;
                    int currentIndex = sites.Count;

                    if (newIndex < currentIndex)
                    {
                        sites.Clear();
                        currentIndex = 0;
                        Pages.Menu.ListViewMenu.Items.Clear();
                    }

                    for (var site = 1; site != newIndex + 1; ++site)
                    {
                        var v = value[site - 1];

                        v.Icon = ReplaceVariables.Replace(v.Icon);
                        v.Path = ReplaceVariables.Replace(v.Path);

                        if (site > currentIndex)
                        {
                            sites.Add(v);
                            MenuHelper.AddSite(site - 1);
                        }
                        else if (v != sites[site - 1])
                        {
                            sites[site - 1] = v;
                            MenuHelper.ReloadSite(site - 1);
                        }
                    }

                    var margin = 100;

                    foreach (var (Title, Icon, Path, StartArguments) in sites)
                    {
                        if (string.IsNullOrEmpty(Title))
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
            }

            private static (bool HideMenu, string Path, string StartArguments) singleSite = (false, "", "");
            public static (bool HideMenu, string Path, string StartArguments) SingleSite
            {
                get => singleSite;
                set
                {
                    if (singleSite == value) return;

                    value.Path = ReplaceVariables.Replace(value.Path);

                    singleSite = value;
                }
            }
        }

        public static class Settings
        {
            private static string file = "";
            public static string File
            {
                get => file;
                set
                {
                    if (string.IsNullOrEmpty(value)) value = @"C:\Users\{username}\AppData\Local\HampisProjekte\RH Utensils\{appName}\Settings.json";

                    if (!string.IsNullOrEmpty(file)) return;

                    value = Path.GetFullPath(ReplaceVariables.Replace(value));

                    if (file == value) return;

                    file = value;
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

                        if (string.IsNullOrEmpty(value)) newestVersion = "-";
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

                        if (string.IsNullOrEmpty(value)) newestVersion = "-";
                        else newestVersion = value;
                    }
                }

                private static string versionsHistoryFile = "";
                public static string VersionsHistoryFile
                {
                    get => versionsHistoryFile;
                    set
                    {
                        if (versionsHistoryFile == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) versionsHistoryFile = uriResult.ToString();
                    }
                }
            }
        }
    }
}