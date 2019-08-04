using Main.Wpf.Utilities;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MenuItem = Main.Wpf.Utilities.MenuItem;

namespace Main.Wpf.Pages
{
    public partial class Menu
    {
        public static Grid GridMenu = new Grid();
        public static ToggleButton ToggleMenu = new ToggleButton();
        public static TransitioningContent TrainsitionigContentSlide;
        public static Grid GridCursor = new Grid();
        public static ListView ListViewMenu = new ListView();

        public static bool _loaded;

        public static bool _registered;

        public Menu()
        {
            InitializeComponent();

            GridMenu = _GridMenu;
            ToggleMenu = _ToggleMenu;
            TrainsitionigContentSlide = _TrainsitionigContentSlide;
            GridCursor = _GridCursor;
            ListViewMenu = _ListViewMenu;

            ListViewMenu.ItemsSource = new List<MenuItem>();

            _registered = true;
        }

        private void Menu_Expanded(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            MenuHelper.ChangeMenuState(MenuState.collapsed);
        }

        private void Menu_Collapsed(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;

            MenuHelper.ChangeMenuState(MenuState.expanded);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Config.Informations.Extension.Name) && Config.Informations.Extension.Name != "RH Utensils")
            {
                if (Config.Menu.DefaultMenuState == MenuState.collapsed || string.Equals(JsonHelper.ReadString(Config.Settings.Json, "menuState"), "collapsed", StringComparison.OrdinalIgnoreCase))
                {
                    ToggleMenu.IsChecked = false;

                    if (Application.Current.MainWindow is MainWindow mw)
                    {
                        mw.Menu.Width = 60;
                        GridMenu.Width = 60;

                        mw.Index.Margin = new Thickness(60, 0, 0, 0);
                        mw.IndexGrid.Margin = new Thickness(60, 0, 0, 0);
                    }
                }
            }

            while (!ConfigHelper._loaded) await Task.Delay(100);

            if (!string.IsNullOrEmpty(Config.Informations.Extension.Name) && Config.Informations.Extension.Name != "RH Utensils" && int.TryParse(await XmlHelper.ReadString(Config.File, "selectionIndex").ConfigureAwait(false), out var index) && index - 1 >= 0 && index <= Config.Menu.Sites.Count)
            {
                await MenuHelper.SelectMenuItemAsync(index - 1);
            }
            else
            {
                await MenuHelper.SelectMenuItemAsync(0);
            }

            _loaded = true;
        }

        private async void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded || Config.Menu._changeingSites) return;

            int index = ListViewMenu.SelectedIndex;

            await MenuHelper.SelectMenuItemAsync(index);

            if (index + 1 == Config.Menu.Sites.Count) return;

            if (Config.Informations.Extension.Name == "RH Utensils") return;

            await XmlHelper.SetString(Config.File, "config/selectionIndex", (index + 1).ToString());
        }
    }
}