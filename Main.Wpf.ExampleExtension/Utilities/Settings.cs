using System;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro;
using MaterialDesignThemes.Wpf;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class Settings
    {
        private static string _theme = "dark";

        private static string _test;

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

                try
                {
                    if (value == "light")
                    {
                        new PaletteHelper().SetLightDark(false);

                        ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"),
                            ThemeManager.GetAppTheme("BaseLight"));

                        Pages.Settings.ThemeProperty.IsChecked = true;
                    }
                    else
                    {
                        new PaletteHelper().SetLightDark(true);

                        ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("blue"),
                            ThemeManager.GetAppTheme("BaseDark"));

                        Pages.Settings.ThemeProperty.IsChecked = false;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                }
            }
        }

        public static string Test
        {
            get => _test;
            set
            {
                if (_test == value) return;

                if (!bool.TryParse(value, out var result)) return;
                Pages.Settings.TestProperty.IsChecked = result;
                _test = value;
            }
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }
    }
}