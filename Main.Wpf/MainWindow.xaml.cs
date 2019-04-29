using MahApps.Metro.Controls;
using Main.Wpf.Functions;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Panel = System.Windows.Forms.Panel;
using Path = System.IO.Path;

namespace Main.Wpf
{
    public partial class MainWindow : MetroWindow
    {
        public static bool IsAbout;
        private readonly Panel _panel = new Panel();

        private IntPtr _appWin;
        private Process _currentProcess;
        public Grid IndexGrid = new Grid();

        public Frame Menu = new Frame();

        private readonly Rectangle Wipe = new Rectangle();

        public MainWindow()
        {
            InitializeComponent();

            var palette = new PaletteHelper().QueryPalette();
            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];

            Wipe.Fill = new SolidColorBrush(hue.Color);
            Wipe.Margin = new Thickness(0);
            Wipe.Visibility = Visibility.Collapsed;
            System.Windows.Controls.Panel.SetZIndex(Wipe, 100);

            MainGrid.Children.Add(Wipe);
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Informations.Extension.Favicon != "")
                {
                    Uri iconUri = new Uri(Informations.Extension.Favicon, UriKind.Relative);
                    Icon = new BitmapImage(iconUri);
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            if (Informations.Extension.WindowHeight > MinHeight) MinHeight = Informations.Extension.WindowHeight;
            if (Informations.Extension.WindowWidth > MinWidth) MinWidth = Informations.Extension.WindowWidth;

            CenterWindowOnScreen();

            if (File.Exists(Path.Combine(Config.ExtensionsDirectory, Informations.Extension.Name, "updater.exe")))
            {
                Index.Navigate(new Uri("Pages/Update.xaml", UriKind.Relative));
                return;
            }

            if (File.Exists(Path.GetFullPath(@".\updater.exe")))
            {
                Index.Navigate(new Uri("Pages/Update.xaml", UriKind.Relative));
                return;
            }

            await Login();
        }

        public async Task Login()
        {
            if (Functions.Login.SkipLogin)
            {
                await LoadExtensionAsync();
            }
            else if (await Functions.Login.FirstRun.Get())
            {
                Index.Navigate(new Uri("Pages/Login.xaml", UriKind.Relative));
            }
            else if (await Functions.Login.LoggedIn.Get())
            {
                Index.Navigate(new Uri("Pages/Login.xaml", UriKind.Relative));
            }
            else
            {
                await LoadExtensionAsync();
            }
        }

        public async Task LoadExtensionAsync(bool wipeAnimation = false)
        {
            if (wipeAnimation) Wipe.Visibility = Visibility.Visible;

            Settings.StartSync();

            var timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Normal, delegate
            {
                if (_appWin != IntPtr.Zero) MoveWindow(_appWin, 0, 0, _panel.Width, _panel.Height, true);
            }, Application.Current.Dispatcher);

            timer.Start();

            if (Functions.Menu.SingleSite.HideMenu)
            {
                //IndexGrid
                MainGrid.Children.Add(IndexGrid);

                //Index
                Index.Visibility = Visibility.Collapsed;

                if (wipeAnimation) await RunWipeAnimation();

                await SetExe(Functions.Menu.SingleSite.Path, Functions.Menu.SingleSite.StartArguments);
            }
            else
            {
                //IndexGrid
                IndexGrid.Margin = new Thickness(250, 0, 0, 0);
                MainGrid.Children.Add(IndexGrid);

                //Index
                Index.Margin = new Thickness(250, 0, 0, 0);
                Index.Visibility = Visibility.Collapsed;

                //Menu
                Menu.HorizontalAlignment = HorizontalAlignment.Left;
                Menu.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                Menu.Navigate(new Uri("Pages/Menu.xaml", UriKind.Relative));
                Menu.Width = 250;
                MainGrid.Children.Add(Menu);

                if (wipeAnimation) await RunWipeAnimation();
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        private async Task WaitForExtension(Process p)
        {
            while (string.IsNullOrEmpty(p.MainWindowTitle))
            {
                await Task.Delay(100);
                p.Refresh();
            }

            return;
        }

        public static bool _loadingEXE;

        public async Task SetExe(string exe, string argument)
        {
            if (_loadingEXE) return;

            _loadingEXE = true;

            var host = new WindowsFormsHost();

            Index.Visibility = Visibility.Collapsed;
            IndexGrid.Visibility = Visibility.Visible;

            Pages.Menu.ListViewMenu.IsEnabled = false;

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

                await WaitForExtension(p);

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
                LogFile.WriteLog(ex);
                Index.Navigate(new Uri("Pages/Error.xaml", UriKind.Relative));
                Index.Visibility = Visibility.Visible;
                IndexGrid.Visibility = Visibility.Collapsed;

                _currentProcess = null;
            }

            Pages.Menu.ListViewMenu.IsEnabled = true;

            _loadingEXE = false;
        }

        public void CenterWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = Width;
            var windowHeight = Height;
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async Task RunWipeAnimation()
        {
            DoubleAnimation Opacity = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(Opacity, new PropertyPath(OpacityProperty));

            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(sb, Wipe);

            sb.Children.Add(Opacity);

            sb.Begin();

            await Task.Delay(300);

            Wipe.Visibility = Visibility.Collapsed;

            sb.Stop();
        }
    }
}