using System;
using System.Collections.Generic;
using System.Linq;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class Config
    {
        private static string _name = "";

        private static string _color = "";

        public static string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrEmpty(_name)) return;

                if (_name == value || string.IsNullOrEmpty(value)) return;

                _name = value;
            }
        }

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
                    new PaletteHelper().ReplacePrimaryColor(color);
                    new PaletteHelper().ReplaceAccentColor(color);

                    for (var i = 0; i < colors.Count; i++)
                        if (Color.Equals(colors[i], StringComparison.OrdinalIgnoreCase))
                            Pages.Settings.ColorProperty.SelectedIndex = i;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                }
            }
        }
    }
}