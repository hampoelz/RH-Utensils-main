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
        private static string file = "";

        public static string File
        {
            get => file;
            set
            {
                value = Path.GetFullPath(ReplaceVariables.Replace(value));

                if (file == value || string.IsNullOrEmpty(value)) return;
                if (!System.IO.File.Exists(value)) return;
                if (!ValidationHelper.IsXmlValid(value)) return;

                file = value;

                ConfigHelper.CreateConfigWatcher();

                LogFile.WriteLog("Change Config File path ...");
            }
        }

        private static string extensionsDirectory = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)), "Extensions");

        public static string ExtensionsDirectory
        {
            get => extensionsDirectory;
            set
            {
                value = Path.GetFullPath(value);

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
                if (extensionDirectoryName == value || string.IsNullOrEmpty(value)) return;

                extensionDirectoryName = value;

                //LogFile.WriteLog("Set Extension Directory Name ...");
            }
        }

        public static class Auth0
        {
            private static string domain = "hampoelz.eu.auth0.com";

            public static string Domain
            {
                get => domain;
                set
                {
                    if (domain == value || string.IsNullOrEmpty(value)) return;

                    domain = value;

                    LogFile.WriteLog("Change Auth0 Domain ...");
                }
            }

            private static string clientId = "_9ZvrbGJUX4MfWdzt6F7pW2e0Z0Zc0OA";

            public static string ClientId
            {
                get => clientId;
                set
                {
                    if (clientId == value || string.IsNullOrEmpty(value)) return;

                    clientId = value;

                    LogFile.WriteLog("Change Auth0 Client ID ...");
                }
            }

            private static string apiClientId = "GTgQvzJvhsSPT0w8sirtIj69cTwfS9AW";

            public static string ApiClientId
            {
                get => apiClientId;
                set
                {
                    if (apiClientId == value || string.IsNullOrEmpty(value)) return;

                    apiClientId = value;

                    LogFile.WriteLog("Change Auth0 API Client ID ...");
                }
            }

            private static string apiClientSecret = "J4db362UcFbgrQBaXb0doKt4MNEjyPh4W2kueckfCpEppl2zHzB8xyLu3N7REknh";

            public static string ApiClientSecret
            {
                get => apiClientSecret;
                set
                {
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
                private static string organisation = "Hampis Projekte";

                public static string Organisation
                {
                    get => organisation;
                    set
                    {
                        if (organisation == value || string.IsNullOrEmpty(value)) return;

                        organisation = value;
                    }
                }

                private static string website = "https://hampoelz.net/";

                public static string Website
                {
                    get => website;
                    set
                    {
                        if (website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                    }
                }
            }

            public static class Developer
            {
                private static string organisation = "RH Utensils";

                public static string Organisation
                {
                    get => organisation;
                    set
                    {
                        if (organisation == value || string.IsNullOrEmpty(value)) return;

                        organisation = value;
                    }
                }

                private static string website = "https://rh-utensils.hampoelz.net/";

                public static string Website
                {
                    get => website;
                    set
                    {
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
                        if (windowHeight == value) return;

                        windowHeight = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinHeight = value;
                    }
                }

                private static int windowWidth = 1200;

                public static int WindowWidth
                {
                    get => windowWidth;
                    set
                    {
                        if (windowWidth == value) return;

                        windowWidth = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;
                        mw.MinWidth = value;
                    }
                }

                private static string name = "RH Utensils";

                public static string Name
                {
                    get => name;
                    set
                    {
                        if (name == value || string.IsNullOrEmpty(value)) return;

                        if (name != "RH Utensils") return;

                        name = value;

                        if (!(Application.Current.MainWindow is MainWindow mw)) return;

                        LogFile.WriteLog("Update app name ...");

                        var oldTitle = mw.Title;

                        string[] Title = oldTitle.Split(new[] { " - " }, StringSplitOptions.None);

                        var newTitle = Title[0] + " - " + value;

                        mw.Title = newTitle;
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

                        LogFile.WriteLog("Update app color ...");

                        try
                        {
                            var Color = new SwatchesProvider().Swatches.FirstOrDefault(a => a.Name == value);
                            new PaletteHelper().ReplacePrimaryColor(Color);
                            new PaletteHelper().ReplaceAccentColor(Color);

                            var palette = new PaletteHelper().QueryPalette();
                            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];
                            Pages.Menu.GridCursor.Background = new SolidColorBrush(hue.Color);
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
                            if (favicon != "")
                            {
                                Uri iconUri = new Uri(favicon, UriKind.Relative);
                                mw.Icon = new BitmapImage(iconUri);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogFile.WriteLog(ex);
                        }
                    }
                }

                private static string sourceCode = "https://github.com/rh-utensils/main";

                public static string SourceCode
                {
                    get => sourceCode;
                    set
                    {
                        if (sourceCode == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) sourceCode = uriResult.ToString();
                    }
                }

                private static string website = "https://github.com/rh-utensils/main";

                public static string Website
                {
                    get => website;
                    set
                    {
                        if (website == value || string.IsNullOrEmpty(value)) return;

                        if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                    }
                }

                private static string issueTracker = "https://github.com/rh-utensils/main/issues/new?assignees=&labels=bug&template=fehlerbericht.md&title=";

                public static string IssueTracker
                {
                    get => issueTracker;
                    set
                    {
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
                get
                {
                    return skipLogin;
                }
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

                    ConfigHelper._configIsChanging = true;
                    await XmlHelper.SetString(Config.File, "config/loggedIn", value.ToString()).ConfigureAwait(false);
                    ConfigHelper._configIsChanging = false;
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

                    ConfigHelper._configIsChanging = true;
                    await XmlHelper.SetString(Config.File, "config/firstRun", value.ToString()).ConfigureAwait(false);
                    ConfigHelper._configIsChanging = false;
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
                get { return defaultMenuState; }
                set
                {
                    if (defaultMenuState == value) return;

                    defaultMenuState = value;
                }
            }

            private static bool isIndexLoading;

            public static bool IsIndexLoading
            {
                get { return isIndexLoading; }
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

            private static List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();

            public static List<(string Title, string Icon, string Path, string StartArguments)> Sites
            {
                get { return sites; }
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
                get { return singleSite; }
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

                    if (file != "") return;

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