using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Main.Wpf.Pages;
using MaterialDesignThemes.Wpf;

namespace Main.Wpf.Utilities
{
    public enum MenuState
    {
        Expanded,
        Collapsed
    }

    public class MenuItem
    {
        private bool _space;
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrEmpty(value)) Space = true;

                _title = value;
            }
        }

        public PackIconKind Icon { get; set; }
        public string Path { get; set; }
        public string StartArguments { get; set; }
        public bool LoadAtStartup { get; set; }

        public bool Space
        {
            get => _space;
            set
            {
                if (value)
                {
                    Height = 20;
                    IsEnabled = false;
                }
                else
                {
                    Height = 60;
                    IsEnabled = true;
                }

                _space = value;
            }
        }

        public int Height { get; set; } = 60;
        public bool IsEnabled { get; set; } = true;
        public bool IsSelected { get; set; }
    }

    public static class MenuHelper
    {
        private static MenuItem _lastSelectedItem;

        public static void ChangeMenuState(MenuState state)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            DoubleAnimation da;
            ThicknessAnimation ta;

            switch (state)
            {
                case MenuState.Collapsed:
                    da = new DoubleAnimation(60, 250, TimeSpan.FromMilliseconds(500));
                    mw.Menu.BeginAnimation(FrameworkElement.WidthProperty, da);
                    Menu.GridMenu.BeginAnimation(FrameworkElement.WidthProperty, da);

                    ta = new ThicknessAnimation(new Thickness(60, 0, 0, 0), new Thickness(250, 0, 0, 0),
                        TimeSpan.FromMilliseconds(500));
                    mw.Index.BeginAnimation(FrameworkElement.MarginProperty, ta);
                    mw.IndexGrid.BeginAnimation(FrameworkElement.MarginProperty, ta);

                    Config.Settings.Json = JsonHelper.ChangeValue(Config.Settings.Json, "menuState", "expanded");
                    break;

                case MenuState.Expanded:
                    da = new DoubleAnimation(250, 60, TimeSpan.FromMilliseconds(500));
                    mw.Menu.BeginAnimation(FrameworkElement.WidthProperty, da);
                    Menu.GridMenu.BeginAnimation(FrameworkElement.WidthProperty, da);

                    ta = new ThicknessAnimation(new Thickness(250, 0, 0, 0), new Thickness(60, 0, 0, 0),
                        TimeSpan.FromMilliseconds(500));
                    mw.Index.BeginAnimation(FrameworkElement.MarginProperty, ta);
                    mw.IndexGrid.BeginAnimation(FrameworkElement.MarginProperty, ta);

                    Config.Settings.Json = JsonHelper.ChangeValue(Config.Settings.Json, "menuState", "collapsed");
                    break;
            }
        }

        public static MenuState StringToMenuState(string state)
        {
            if (!Enum.IsDefined(typeof(MenuState), state.ToLower())) return MenuState.Expanded;

            return (MenuState) Enum.Parse(typeof(MenuState), state.ToLower());
        }

        [Obsolete]
        public static async Task SelectMenuItemAsync(int index)
        {
            try
            {
                while (!ConfigHelper._loaded) await Task.Delay(100);

                var sites = Config.Menu.Sites;

                if (index < 0 || index > sites.Count) return;

                var menuItem = sites[index];

                if (menuItem.Path == "account.exe")
                {
                    if (await Config.Login.LoggedIn.Get())
                    {
                        await AccountHelper.Client.LogoutAsync();

                        await Config.Login.LoggedIn.Set(false);
                        await Config.Login.FirstRun.Set(true);

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly()?.Location)
                        {
                            Arguments = string.Join(" ", App.Parameters)
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }
                    else
                    {
                        await Config.Login.LoggedIn.Set(true);

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly()?.Location)
                        {
                            Arguments = string.Join(" ", App.Parameters)
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }

                    return;
                }

                if (menuItem == _lastSelectedItem) return;

                MoveCursorMenu(index);
                Menu.ListViewMenu.SelectedItems.Clear();

                foreach (var item in sites)
                    if (item == menuItem)
                        item.IsSelected = true;

                await Config.Menu.SetSites(sites);

                if (!(Application.Current.MainWindow is MainWindow mw)) return;

                switch (sites[index].Path)
                {
                    case "selector.exe":
                        IndexHelper.SetIndex("Selector");
                        break;

                    case "info.exe":
                        IndexHelper.SetIndex("About");
                        break;

                    default:
                        await mw.SetExe(sites[index].Path, sites[index].StartArguments, index);
                        break;
                }

                mw.Title = sites[index].Title + " - " + Config.Informations.Extension.Name;

                _lastSelectedItem = menuItem;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        public static void MoveCursorMenu(int index)
        {
            var margin = 100;

            var sites = Config.Menu.Sites;

            for (var i = 0; i < index; ++i)
                if (string.IsNullOrEmpty(sites[i].Title) ||
                    string.Equals(sites[i].Title, "null", StringComparison.OrdinalIgnoreCase) || sites[i].Title == null)
                    margin += 20;
                else
                    margin += 60;

            Menu.TrainsitionigContentSlide.OnApplyTemplate();
            Menu.GridCursor.Margin = new Thickness(0, margin, 0, 0);
        }
    }
}