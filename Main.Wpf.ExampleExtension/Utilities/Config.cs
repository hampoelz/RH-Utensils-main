using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class Config
    {
        private static string name = "";
        public static string Name
        {
            get => name;
            set
            {
                if (!string.IsNullOrEmpty(name)) return;

                if (name == value || string.IsNullOrEmpty(value)) return;

                name = value;
            }
        }

        private static string color = "";
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
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                }
            }
        }
    }
}
