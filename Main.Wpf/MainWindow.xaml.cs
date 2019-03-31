using Main.Wpf.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Panel = System.Windows.Forms.Panel;

namespace Main.Wpf
{
    public partial class MainWindow
    {
        public static bool IsAbout;
        private readonly Panel _panel = new Panel();

        private IntPtr _appWin;
        private Process _currentProcess;
        public Grid IndexGrid = new Grid();

        public Frame Menu = new Frame();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ExtensionName != "") Functions.Settings.SyncTheme();

            if (App.Favicon != "")
                Icon = new BitmapImage(new Uri(App.Favicon));

            if (!App.HideMenu)
            {
                var margin = 100;

                foreach (var t in App.SitesTitles)
                    if (t == "")
                        margin += 20;
                    else
                        margin += 60;

                if (margin > 420) MinHeight = 640 + margin - 420;
            }

            Height = MinHeight;
            Width = MinWidth;

            CenterWindowOnScreen();

            if (File.Exists(App.ExtensionsDirectory + @"\" + App.ExtensionName + @"\" + "updater.exe"))
            {
                Index.Navigate(new Uri("Pages/Update.xaml", UriKind.Relative));
                return;
            }

            if (File.Exists(@".\updater.exe"))
            {
                Index.Navigate(new Uri("Pages/Update.xaml", UriKind.Relative));
                return;
            }

            await Login();
        }

        public async Task Login()
        {
            if (App.SkipLogin || Settings.Default.skipLogin && !Settings.Default.login && !App.ShowLogin &&
                !App.ShowFirstPage)
                await LoadExtensionAsync();
            else
                Index.Navigate(new Uri("Pages/Login.xaml", UriKind.Relative));

            if (App.ExtensionName != "") Functions.Settings.CreateFileWatcher(App.SettingsFile);
        }

        public async Task LoadExtensionAsync()
        {
            if (App.HideMenu)
            {
                //IndexGrid
                Grid.Children.Add(IndexGrid);

                //Index
                Index.Visibility = Visibility.Collapsed;

                await Functions.Index.SetExeAsync(App.Exe, App.ExeArguments, App.ExeLoadTime);
            }
            else
            {
                //IndexGrid
                IndexGrid.Margin = new Thickness(250, 0, 0, 0);
                Grid.Children.Add(IndexGrid);

                var timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    if (_appWin != IntPtr.Zero) MoveWindow(_appWin, 0, 0, _panel.Width, _panel.Height, true);
                }, Application.Current.Dispatcher);

                timer.Start();

                //Index
                Index.Margin = new Thickness(250, 0, 0, 0);
                Index.Visibility = Visibility.Collapsed;

                //Menu
                Menu.HorizontalAlignment = HorizontalAlignment.Left;
                Menu.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                Menu.Navigate(new Uri("Pages/Menu.xaml", UriKind.Relative));
                Menu.Width = 250;
                Grid.Children.Add(Menu);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        public async Task SetExe(string exe, string argument, int loadTime)
        {
            var host = new WindowsFormsHost();

            Index.Visibility = Visibility.Collapsed;
            IndexGrid.Visibility = Visibility.Visible;

            Pages.Menu.IsIndexLoading = true;

            try
            {
                Index.Navigate(new Uri("Pages/Load.xaml", UriKind.Relative));
                Index.Visibility = Visibility.Visible;
                IndexGrid.Visibility = Visibility.Collapsed;

                var ps = new ProcessStartInfo(exe)
                {
                    Arguments = argument,

                    WindowStyle = ProcessWindowStyle.Minimized
                };

                _currentProcess?.Kill();

                var p = Process.Start(ps);

                _currentProcess = p;

                await Task.Delay(loadTime);
                //Thread.Sleep(loadTime);

                Index.Visibility = Visibility.Collapsed;
                IndexGrid.Visibility = Visibility.Visible;

                if (p != null) _appWin = p.MainWindowHandle;

                SetParent(_appWin, _panel.Handle);

                ps.WindowStyle = ProcessWindowStyle.Maximized;

                host.Child = _panel;

                IndexGrid.Children.Add(host);

                if (_appWin != IntPtr.Zero) MoveWindow(_appWin, 0, 0, _panel.Width, _panel.Height, true);
            }
            catch (Exception ex)
            {
                Index.Navigate(new Uri("Pages/Error.xaml", UriKind.Relative));
                Index.Visibility = Visibility.Visible;
                IndexGrid.Visibility = Visibility.Collapsed;

                Pages.Error.ErrorMessage = ex.ToString();
                Pages.Error.Title = "Fehler beim Laden einer Seite";
                Pages.Error.File = exe;

                _currentProcess = null;
            }

            Pages.Menu.IsIndexLoading = false;
        }

        public void CenterWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = Width;
            var windowHeight = Height;
            Left = screenWidth / 2 - windowWidth / 2;
            Top = screenHeight / 2 - windowHeight / 2;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}