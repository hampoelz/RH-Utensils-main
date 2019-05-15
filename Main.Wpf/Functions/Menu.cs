using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Main.Wpf.Functions
{
    public enum MenuState { expanded, collapsed };

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

        public static MenuState StringToMenuState(string state)
        {
            if (!Enum.IsDefined(typeof(MenuState), state.ToLower())) return MenuState.expanded;

            return (MenuState)Enum.Parse(typeof(MenuState), state.ToLower());
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

        private static readonly List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();

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
                        AddSite(site - 1);
                    }
                    else if (v != sites[site - 1])
                    {
                        sites[site - 1] = v;
                        ReloadSite(site - 1);
                    }
                }

                var margin = 100;

                foreach (var (Title, Icon, Path, StartArguments) in sites)
                {
                    if (Title?.Length == 0)
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

        private static void AddSite(int index)
        {
            LogFile.WriteLog("Add page ...");

            try
            {
                var menuItem = new ListViewItem();
                Pages.Menu.ListViewMenu.Items.Add(menuItem);

                var menuItemStack = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                var menuItemIcon = new Image
                {
                    Width = 30,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10)
                };

                var menuItemText = new TextBlock
                {
                    FontSize = 17,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(20, 0, 20, 0)
                };

                menuItem.Content = menuItemStack;
                menuItemStack.Children.Add(menuItemIcon);
                menuItemStack.Children.Add(menuItemText);

                menuItem.Selected += Menu_Selected;

                ReloadSite(index);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static void ReloadSite(int index)
        {
            LogFile.WriteLog("Update page ...");

            try
            {
                if (sites.Count < index) return;

                var items = Pages.Menu.ListViewMenu.Items;

                ListViewItem menuItem = null;

                foreach (var item in items)
                {
                    if (!(item is ListViewItem tempMenuItem)) continue;
                    if (items[index] != tempMenuItem) continue;

                    menuItem = tempMenuItem;
                    break;
                }

                if (menuItem == null) return;

                var menuItemStack = menuItem.Content as StackPanel;

                Image menuItemIcon = null;

                foreach (var item in menuItemStack.Children)
                {
                    if (!(item is Image tempMenuItemIcon)) continue;

                    menuItemIcon = tempMenuItemIcon;
                    break;
                }

                if (menuItemIcon == null) return;

                TextBlock menuItemText = null;

                foreach (var item in menuItemStack.Children)
                {
                    if (!(item is TextBlock tempMenuItemText)) continue;

                    menuItemText = tempMenuItemText;
                    break;
                }

                if (menuItemText == null) return;

                if (sites[index].Title?.Length == 0 || string.Equals(sites[index].Title, "null", StringComparison.OrdinalIgnoreCase) || sites[index].Title == null)
                {
                    if (index == 0) return;

                    menuItem.Height = 20;
                    menuItem.IsEnabled = false;
                    menuItem.Name = null;

                    menuItemIcon.Source = null;

                    menuItemText.Text = null;
                }
                else
                {
                    menuItem.Height = 60;
                    menuItem.IsEnabled = true;
                    menuItem.Name = "MenuItem_" + index;

                    try
                    {
                        menuItemIcon.Source = new BitmapImage(new Uri(sites[index].Icon));
                    }
                    catch
                    {
                        menuItemIcon.Source = new BitmapImage(new Uri("/Assets/application.png", UriKind.Relative));
                    }

                    menuItemText.Text = sites[index].Title;

                    switch (sites[index].Path)
                    {
                        case "account.exe" when Login.LoggedIn.Get().Result:
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
                }

                if (index == Pages.Menu.ListViewMenu.SelectedIndex)
                {
                    if (!(Application.Current.MainWindow is MainWindow mw)) return;
                    mw.Title = sites[index].Title + " - " + Informations.Extension.Name;
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static ListViewItem _lastSelected;

        private static async void Menu_Selected(object sender, RoutedEventArgs e)
        {
            if (!Pages.Menu._loaded) return;

            var menuItem = (ListViewItem)sender;

            var index = int.Parse(menuItem.Name.Replace("MenuItem_", ""));

            await SelectMenuItemAsync(index).ConfigureAwait(false);

            if (index + 1 == sites.Count) return;

            if (Informations.Extension.Name == "RH Utensils") return;

            Config._isChanging = true;
            await Xml.SetString(Config.File, "config/selectionIndex", (index + 1).ToString());
            Config._isChanging = false;
        }

        public static async Task SelectMenuItemAsync(int index)
        {
            try
            {
            start:

                ItemCollection items = Pages.Menu.ListViewMenu.Items;

                ListViewItem menuItem = null;

                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    foreach (var item in items)
                    {
                        if (!(item is ListViewItem tempMenuItem)) continue;
                        if (tempMenuItem.Name != "MenuItem_" + index.ToString()) continue;

                        menuItem = tempMenuItem;
                        break;
                    }
                }));

                if (menuItem == null)
                {
                    index = 0;
                    goto start;
                }

                if (sites[index].Path == "account.exe")
                {
                    if (await Login.LoggedIn.Get())
                    {
                        await Account.Client.LogoutAsync();

                        await Login.LoggedIn.Set(false);
                        await Login.FirstRun.Set(true);

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                        {
                            Arguments = string.Join(" ", App.Parameters)
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }
                    else
                    {
                        await Login.LoggedIn.Set(true);

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                        {
                            Arguments = string.Join(" ", App.Parameters)
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }

                    menuItem.IsSelected = false;

                    _lastSelected.IsSelected = true;

                    return;
                }

                if (menuItem == _lastSelected) return;

                MoveCursorMenu(index);
                Pages.Menu.ListViewMenu.SelectedItems.Clear();

                menuItem.IsSelected = true;

                if (!(Application.Current.MainWindow is MainWindow mw)) return;

                switch (sites[index].Path)
                {
                    case "selector.exe":
                        Index.Set("Selector");
                        break;

                    case "info.exe":
                        Index.Set("About");
                        break;

                    default:
                        await mw.SetExe(sites[index].Path, sites[index].StartArguments.Replace("{fileAssociation}", Versioning.File), index);
                        break;
                }

                mw.Title = sites[index].Title + " - " + Informations.Extension.Name;

                _lastSelected = menuItem;
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static void MoveCursorMenu(int index)
        {
            var margin = 100;

            for (var i = 0; i < index; ++i)
            {
                if (sites[i].Title?.Length == 0 || string.Equals(sites[i].Title, "null", StringComparison.OrdinalIgnoreCase) || sites[i].Title == null)
                {
                    margin += 20;
                }
                else
                {
                    margin += 60;
                }
            }

            Pages.Menu.TrainsitionigContentSlide.OnApplyTemplate();
            Pages.Menu.GridCursor.Margin = new Thickness(0, margin, 0, 0);
        }
    }
}