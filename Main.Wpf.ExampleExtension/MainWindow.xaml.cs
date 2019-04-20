using Main.Wpf.ExampleExtension.Functions;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Main.Wpf.ExampleExtension
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Config.File = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

            string[] args = Environment.GetCommandLineArgs();

            for (var arg = 1; arg != args.Length; ++arg)
            {
                switch (args[arg])
                {
                    case "-page":
                        Functions.Index.Set(args[arg + 1]);
                        continue;

                        //other cases
                }

                if (File.Exists(args[arg]))
                {
                    App.File = args[arg];
                    continue;
                }
            }

            var configColor = await Config.ReadString("color").ConfigureAwait(false);

            var Color = new SwatchesProvider().Swatches.FirstOrDefault(a => a.Name == configColor.ToLower());
            new PaletteHelper().ReplacePrimaryColor(Color);
            new PaletteHelper().ReplaceAccentColor(Color);
        }
    }
}