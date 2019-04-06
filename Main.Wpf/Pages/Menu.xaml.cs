using Main.Wpf.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Main.Wpf.Pages
{
    public partial class Menu
    {
        public static bool IsIndexLoading = false;
        private readonly bool _toggleMenuBool;

        private ListViewItem _lastSelected;

        public Menu()
        {
            InitializeComponent();

            LoadMenu();

            if (App.ExtensionName != "")
                if (App.MenuState == "collapsed" || Functions.Json.ConvertToString(App.SettingsJson, "menuState") == "collapsed")
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

            _toggleMenuBool = true;

            var timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Normal, delegate
            {
                if (IsIndexLoading)
                    ListViewMenu.IsEnabled = false;
                else if (!IsIndexLoading) ListViewMenu.IsEnabled = true;
            }, Application.Current.Dispatcher);

            timer.Start();
        }

        private void ToggleMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (!_toggleMenuBool) return;

            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var da = new DoubleAnimation(60, 250, TimeSpan.FromMilliseconds(500));
            mw.Menu.BeginAnimation(WidthProperty, da);
            GridMenu.BeginAnimation(WidthProperty, da);

            var ta = new ThicknessAnimation(new Thickness(60, 0, 0, 0), new Thickness(250, 0, 0, 0),
                TimeSpan.FromMilliseconds(500));
            mw.Index.BeginAnimation(MarginProperty, ta);
            mw.IndexGrid.BeginAnimation(MarginProperty, ta);

            Functions.Settings.Set(Functions.Json.ChangeValue(App.SettingsJson, "menuState", "expanded"));
        }

        private void ToggleMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_toggleMenuBool) return;

            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var da = new DoubleAnimation(250, 60, TimeSpan.FromMilliseconds(500));
            mw.Menu.BeginAnimation(WidthProperty, da);
            GridMenu.BeginAnimation(WidthProperty, da);

            var ta = new ThicknessAnimation(new Thickness(250, 0, 0, 0), new Thickness(60, 0, 0, 0),
                TimeSpan.FromMilliseconds(500));
            mw.Index.BeginAnimation(MarginProperty, ta);
            mw.IndexGrid.BeginAnimation(MarginProperty, ta);

            Functions.Settings.Set(Functions.Json.ChangeValue(App.SettingsJson, "menuState", "collapsed"));
        }

        private void LoadMenu()
        {
            try
            {
                for (var i = 0; i < App.SitesTitles.Count; ++i)
                {
                    var menuItem = new ListViewItem();
                    ListViewMenu.Items.Add(menuItem);

                    if (App.SitesTitles[i] == "")
                    {
                        if (i == 0)
                        {
                            Functions.Index.SetError("Placeholder can't be placed at the top of the menu!",
                                "Fehler beim Laden des Navigationsmenüs");
                            return;
                        }

                        menuItem.Height = 20;
                        menuItem.IsEnabled = false;
                    }
                    else
                    {
                        menuItem.Height = 60;
                        menuItem.Selected += Menu_Selected;
                        menuItem.Name = "MenuItem_" + i;

                        var menuItemStack = new StackPanel
                        {
                            Orientation = Orientation.Horizontal
                        };

                        menuItem.Content = menuItemStack;

                        var menuItemIcon = new Image();

                        try
                        {
                            menuItemIcon.Source = new BitmapImage(new Uri(App.SitesIcons[i]));
                        }
                        catch
                        {
                            menuItemIcon.Source = new BitmapImage(new Uri("/Assets/application.png", UriKind.Relative));
                        }

                        menuItemIcon.Width = 30;
                        menuItemIcon.VerticalAlignment = VerticalAlignment.Center;
                        menuItemIcon.Margin = new Thickness(10);

                        var menuItemText = new TextBlock
                        {
                            Text = App.SitesTitles[i],
                            FontSize = 17,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(20, 0, 20, 0)
                        };

                        switch (App.SitesPaths[i])
                        {
                            case "account.exe" when Settings.Default.login:
                                menuItemIcon.Source = new BitmapImage(new Uri("/Assets/logout.png", UriKind.Relative));
                                menuItemText.Text = "Abmelden";
                                break;

                            case "account.exe":
                                menuItemIcon.Source = new BitmapImage(new Uri("/Assets/login.png", UriKind.Relative));
                                menuItemText.Text = "Anmelden";
                                break;

                            case "info.exe":
                                menuItemIcon.Source = new BitmapImage(new Uri("/Assets/information.png", UriKind.Relative));
                                menuItemText.Text = "Information";
                                break;

                            case "selector.exe":
                                menuItemIcon.Source =
                                    new BitmapImage(new Uri("/Assets/extensions.png", UriKind.Relative));
                                break;
                        }

                        menuItemStack.Children.Add(menuItemIcon);
                        menuItemStack.Children.Add(menuItemText);

                        if (i == 0)
                        {
                            menuItem.IsSelected = true;
                            _lastSelected = menuItem;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.Index.SetError(ex.ToString(), "Fehler beim Laden des Navigationsmenüs");
            }
        }

        private async void Menu_Selected(object sender, RoutedEventArgs e)
        {
            About._timer.Stop();
            try
            {
                var menu = (ListViewItem)sender;

                var menuItem = int.Parse(menu.Name.Replace("MenuItem_", ""));

                if (App.SitesPaths[menuItem] == "account.exe")
                {
                    if (Settings.Default.login)
                    {
                        await Functions.Account.Client.LogoutAsync();

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                        {
                            Arguments = "-firstPage " + App.Parameters
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }
                    else
                    {
                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                        {
                            Arguments = "-login " + App.Parameters
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }

                    menu.IsSelected = false;

                    _lastSelected.IsSelected = true;

                    return;
                }

                if (menu == _lastSelected) return;

                if (_lastSelected != null)
                {
                    MoveCursorMenu(menuItem);

                    _lastSelected.IsSelected = false;
                }

                if (App.SitesPaths[menuItem] == "selector.exe")
                    Functions.Index.Set("Selector");
                else if (App.SitesPaths[menuItem] == "info.exe")
                    Functions.Index.Set("About");
                else
                    await Functions.Index.SetExeAsync(App.SitesPaths[menuItem], App.SitesPathsArguments[menuItem],
                        App.SitesLoadingTimes[menuItem]);

                if (Application.Current.MainWindow is MainWindow mw)
                    mw.Title = App.SitesTitles[menuItem] + " - " + App.Name;

                _lastSelected = menu;
            }
            catch (Exception ex)
            {
                Functions.Index.SetError(ex.ToString(), "Fehler beim Auswählen des Navigationsmenüs");
            }
        }

        private void MoveCursorMenu(int index)
        {
            var margin = 100;

            for (var i = 0; i < index; ++i)
                if (App.SitesTitles[i] == "")
                    margin += 20;
                else
                    margin += 60;

            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, margin, 0, 0);
        }
    }
}