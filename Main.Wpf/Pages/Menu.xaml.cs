using Main.Wpf.Functions;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Main.Wpf.Pages
{
    public partial class Menu
    {
        public static TransitioningContent TrainsitionigContentSlide = new TransitioningContent();
        public static Grid GridCursor = new Grid();
        public static ListView ListViewMenu = new ListView();

        public static bool _loaded;

        public Menu()
        {
            InitializeComponent();

            TrainsitionigContentSlide.OpeningEffect = new TransitionEffect(TransitionEffectKind.SlideInFromLeft, TimeSpan.FromSeconds(0.2));

            GridCursor.Margin = new Thickness(0, 100, 0, 0);
            var palette = new PaletteHelper().QueryPalette();
            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];
            GridCursor.Background = new SolidColorBrush(hue.Color);
            GridCursor.Width = 10;
            GridCursor.HorizontalAlignment = HorizontalAlignment.Left;
            GridCursor.Height = 60;
            GridCursor.VerticalAlignment = VerticalAlignment.Top;

            MainGrid.Children.Add(TrainsitionigContentSlide);
            TrainsitionigContentSlide.Content = GridCursor;

            ListViewMenu.Margin = new Thickness(0, 100, 0, 100);
            ListViewMenu.Foreground = Brushes.LightGray;
            ListViewMenu.FontSize = 18;
            ListViewMenu.HorizontalAlignment = HorizontalAlignment.Left;
            ListViewMenu.Width = 250;
            MainGrid.Children.Add(ListViewMenu);
        }

        private void ToggleMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var da = new DoubleAnimation(60, 250, TimeSpan.FromMilliseconds(500));
            mw.Menu.BeginAnimation(WidthProperty, da);
            MainGrid.BeginAnimation(WidthProperty, da);

            var ta = new ThicknessAnimation(new Thickness(60, 0, 0, 0), new Thickness(250, 0, 0, 0),
                TimeSpan.FromMilliseconds(500));
            mw.Index.BeginAnimation(MarginProperty, ta);
            mw.IndexGrid.BeginAnimation(MarginProperty, ta);

            Settings.Json = Json.ChangeValue(Settings.Json, "menuState", "expanded");
        }

        private void ToggleMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var da = new DoubleAnimation(250, 60, TimeSpan.FromMilliseconds(500));
            mw.Menu.BeginAnimation(WidthProperty, da);
            MainGrid.BeginAnimation(WidthProperty, da);

            var ta = new ThicknessAnimation(new Thickness(250, 0, 0, 0), new Thickness(60, 0, 0, 0),
                TimeSpan.FromMilliseconds(500));
            mw.Index.BeginAnimation(MarginProperty, ta);
            mw.IndexGrid.BeginAnimation(MarginProperty, ta);

            Settings.Json = Json.ChangeValue(Settings.Json, "menuState", "collapsed");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Informations.Extension.Name != "" && Informations.Extension.Name != "RH Utensils")
            {
                if (Functions.Menu.DefaultMenuState == MenuState.collapsed || string.Equals(Json.ReadString(Settings.Json, "menuState"), "collapsed", StringComparison.OrdinalIgnoreCase))
                {
                    ToggleMenu.IsChecked = false;

                    if (Application.Current.MainWindow is MainWindow mw)
                    {
                        mw.Menu.Width = 60;
                        MainGrid.Width = 60;

                        mw.Index.Margin = new Thickness(60, 0, 0, 0);
                        mw.IndexGrid.Margin = new Thickness(60, 0, 0, 0);
                    }
                }
            }

            if (Informations.Extension.Name != "" && Informations.Extension.Name != "RH Utensils" && int.TryParse(await Xml.ReadString(Config.File, "selectionIndex").ConfigureAwait(false), out var index) && index - 1 >= 0)
            {
                await Functions.Menu.SelectMenuItemAsync(index - 1).ConfigureAwait(false);
            }

            await Functions.Menu.SelectMenuItemAsync(0).ConfigureAwait(false);

            _loaded = true;
        }
    }
}