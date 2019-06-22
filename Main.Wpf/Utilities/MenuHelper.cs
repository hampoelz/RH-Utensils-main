using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Main.Wpf.Utilities
{
    public enum MenuState { expanded, collapsed };

    public static class MenuHelper
    {
        public static MenuState StringToMenuState(string state)
        {
            if (!Enum.IsDefined(typeof(MenuState), state.ToLower())) return MenuState.expanded;

            return (MenuState)Enum.Parse(typeof(MenuState), state.ToLower());
        }

        public static void AddSite(int index)
        {
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

        public static void ReloadSite(int index)
        {
            try
            {
                var sites = Config.Menu.Sites;

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
                        case "account.exe" when Config.Login.LoggedIn.Get().Result:
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
                    mw.Title = sites[index].Title + " - " + Config.Informations.Extension.Name;
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        private static ListViewItem _lastSelectedItem;

        private static async void Menu_Selected(object sender, RoutedEventArgs e)
        {
            if (!Pages.Menu._loaded) return;

            var menuItem = (ListViewItem)sender;

            var index = int.Parse(menuItem.Name.Replace("MenuItem_", ""));

            await SelectMenuItemAsync(index).ConfigureAwait(false);

            if (index + 1 == Config.Menu.Sites.Count) return;

            if (Config.Informations.Extension.Name == "RH Utensils") return;

            await XmlHelper.SetString(Config.File, "config/selectionIndex", (index + 1).ToString());
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

                var sites = Config.Menu.Sites;

                if (sites[index].Path == "account.exe")
                {
                    if (await Config.Login.LoggedIn.Get())
                    {
                        await AccountHelper.Client.LogoutAsync();

                        await Config.Login.LoggedIn.Set(false);
                        await Config.Login.FirstRun.Set(true);

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                        {
                            Arguments = string.Join(" ", App.Parameters)
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }
                    else
                    {
                        await Config.Login.LoggedIn.Set(true);

                        var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                        {
                            Arguments = string.Join(" ", App.Parameters)
                        };
                        Process.Start(ps);

                        Application.Current.Shutdown();
                    }

                    menuItem.IsSelected = false;

                    _lastSelectedItem.IsSelected = true;

                    return;
                }

                if (menuItem == _lastSelectedItem) return;

                MoveCursorMenu(index);
                Pages.Menu.ListViewMenu.SelectedItems.Clear();

                menuItem.IsSelected = true;

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

        private static void MoveCursorMenu(int index)
        {
            var margin = 100;

            var sites = Config.Menu.Sites;

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