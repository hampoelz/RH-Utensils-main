using MahApps.Metro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Main.Wpf.Functions
{
    public static class Informations
    {
        public static class Copyright
        {
            private static string organisation = "Hampis Projekte";

            public static string Organisation
            {
                get { return organisation; }
                set
                {
                    if (organisation == value || value?.Length == 0) return;

                    organisation = value;
                }
            }

            private static string website = "https://hampoelz.net/";

            public static string Website
            {
                get { return website; }
                set
                {
                    if (website == value || value?.Length == 0) return;

                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                }
            }
        }

        public static class Developer
        {
            private static string organisation = "RH Utensils";

            public static string Organisation
            {
                get { return organisation; }
                set
                {
                    if (organisation == value || value?.Length == 0) return;

                    organisation = value;
                }
            }

            private static string website = "https://rh-utensils.hampoelz.net/";

            public static string Website
            {
                get { return website; }
                set
                {
                    if (website == value || value?.Length == 0) return;

                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                }
            }
        }

        public static class Extension
        {
            private static int windowHeight = 700;

            public static int WindowHeight
            {
                get { return windowHeight; }
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
                get { return windowWidth; }
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
                get { return name; }
                set
                {
                    if (name == value || value?.Length == 0) return;

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
                get { return color; }
                set
                {
                    value = value.ToLower();

                    if (color == value || value?.Length == 0) return;

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
                get { return theme; }
                set
                {
                    value = value.ToLower();

                    if (theme == value || value?.Length == 0) return;

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
                get { return favicon; }
                set
                {
                    value = Path.GetFullPath(ReplaceVariables.Replace(value));

                    if (favicon == value || value?.Length == 0) return;

                    if (!Validation.IsImageValid(value)) return;

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
                get { return sourceCode; }
                set
                {
                    if (sourceCode == value || value?.Length == 0) return;

                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) sourceCode = uriResult.ToString();
                }
            }

            private static string website = "https://github.com/rh-utensils/main";

            public static string Website
            {
                get { return website;
                }
                set
                {
                    if (website == value || value?.Length == 0) return;

                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) website = uriResult.ToString();
                }
            }

            private static string issueTracker = "https://github.com/rh-utensils/main/issues/new?assignees=&labels=bug&template=fehlerbericht.md&title=";

            public static string IssueTracker
            {
                get { return issueTracker; }
                set
                {
                    if (issueTracker == value || value?.Length == 0) return;

                    if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                        return;
                    issueTracker = uriResult.ToString();
                }
            }
        }
    }
}