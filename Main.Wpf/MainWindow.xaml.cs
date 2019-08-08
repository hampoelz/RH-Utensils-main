using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Main.Wpf.Utilities;
using MaterialDesignThemes.Wpf;
using Panel = System.Windows.Forms.Panel;
using Path = System.IO.Path;

namespace Main.Wpf
{
    public partial class MainWindow
    {
        public const int SwHide = 0;
        public const int SwShownormal = 1;

        public static bool LoadingExe;

        public static List<(Process proc, int id)> BackgroundProcesses = new List<(Process proc, int id)>();
        private readonly Panel _panel = new Panel();

        private readonly Rectangle _wipe = new Rectangle();

        private IntPtr _appWin;
        public Process CurrentProcess;
        public Grid IndexGrid = new Grid();

        public Frame Menu = new Frame();

        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();

            var palette = new PaletteHelper().QueryPalette();
            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];

            _wipe.Fill = new SolidColorBrush(hue.Color);
            _wipe.Margin = new Thickness(0);
            _wipe.Visibility = Visibility.Collapsed;
            System.Windows.Controls.Panel.SetZIndex(_wipe, 100);

            MainGrid.Children.Add(_wipe);

            Title = Config.Informations.Extension.Name;
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessageHelper.ReceiveDataMessages();

            try
            {
                if (!string.IsNullOrEmpty(Config.Informations.Extension.Favicon))
                {
                    var iconUri = new Uri(Config.Informations.Extension.Favicon, UriKind.Relative);
                    Icon = new BitmapImage(iconUri);
                }

                MinHeight = Config.Informations.Extension.WindowHeight;
                MinWidth = Config.Informations.Extension.WindowWidth;

                CenterWindowOnScreen();

                if (File.Exists(Path.Combine(Config.ExtensionsDirectory, Config.Informations.Extension.Name,
                    "updater.exe")))
                {
                    await UpdateHelper.SetupProgrammUpdate();
                    return;
                }

                if (File.Exists(Path.Combine(
                    Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ??
                    throw new InvalidOperationException(), "updater.exe")))
                {
                    await UpdateHelper.SetupProgrammUpdate();
                    return;
                }

                await Login();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        public async Task Login()
        {
            if (Config.Login.SkipLogin)
                await LoadExtensionAsync();
            else if (await Config.Login.FirstRun.Get())
                Index.Navigate(new Uri("Pages/Login.xaml", UriKind.Relative));
            else if (await Config.Login.LoggedIn.Get())
                Index.Navigate(new Uri("Pages/Login.xaml", UriKind.Relative));
            else
                await LoadExtensionAsync();
        }

        [Obsolete]
        public async Task LoadExtensionAsync(bool wipeAnimation = false)
        {
            try
            {
                if (wipeAnimation) _wipe.Visibility = Visibility.Visible;

                await SettingsHelper.StartSync();

                var timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Normal, delegate
                    {
                        if (_appWin != IntPtr.Zero) MoveWindow(_appWin, 0, 0, _panel.Width, _panel.Height, true);
                    }, Application.Current.Dispatcher ?? throw new InvalidOperationException());

                timer.Start();

                if (Config.Menu.SingleSite.HideMenu)
                {
                    MainGrid.Children.Add(IndexGrid);

                    Index.Visibility = Visibility.Collapsed;

                    if (wipeAnimation) await RunWipeAnimation();

                    await SetExe(Config.Menu.SingleSite.Path, Config.Menu.SingleSite.StartArguments);
                }
                else
                {
                    IndexGrid.Margin = new Thickness(250, 0, 0, 0);
                    MainGrid.Children.Add(IndexGrid);

                    Index.Margin = new Thickness(250, 0, 0, 0);
                    Index.Visibility = Visibility.Collapsed;

                    Menu.HorizontalAlignment = HorizontalAlignment.Left;
                    Menu.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                    Menu.Navigate(new Uri("Pages/Menu.xaml", UriKind.Relative));
                    Menu.Width = 250;
                    MainGrid.Children.Add(Menu);

                    if (wipeAnimation) await RunWipeAnimation();
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static async Task WaitForExtension(Process p)
        {
            while (string.IsNullOrEmpty(p.MainWindowTitle))
            {
                await Task.Delay(100);
                p.Refresh();
            }
        }

        [Obsolete]
        public async Task SetExe(string exe, string argument, int id = -1)
        {
            if (LoadingExe) return;

            LoadingExe = true;

            var host = new WindowsFormsHost();

            Index.Visibility = Visibility.Collapsed;
            IndexGrid.Visibility = Visibility.Visible;

            Pages.Menu.ListViewMenu.IsEnabled = false;

            try
            {
                Index.Navigate(new Uri("Pages/Load.xaml", UriKind.Relative));
                Index.Visibility = Visibility.Visible;
                IndexGrid.Visibility = Visibility.Collapsed;

                exe = ReplaceVariables.Replace(exe);
                argument = string.Equals(argument, "null", StringComparison.OrdinalIgnoreCase) ? "" : argument;

                var ps = new ProcessStartInfo(exe)
                {
                    Arguments = argument,

                    WindowStyle = ProcessWindowStyle.Minimized
                };

                var hideExe = false;

                for (var i = 0; i < BackgroundProcesses.Count; ++i)
                {
                    var proc = BackgroundProcesses[i].proc;

                    if (proc != CurrentProcess) continue;
                    ShowWindow(proc.MainWindowHandle, SwHide);
                    hideExe = true;
                    break;
                }

                if (!hideExe) CurrentProcess?.Kill();

                var p = Process.Start(ps);

                var exeIsHidden = false;

                for (var i = 0; i < BackgroundProcesses.Count; ++i)
                {
                    var proc = BackgroundProcesses[i].proc;
                    var procId = BackgroundProcesses[i].id;

                    if (procId != id || proc.MainModule?.FileName != exe) continue;
                    ShowWindow(proc.MainWindowHandle, SwShownormal);
                    _appWin = proc.MainWindowHandle;
                    p?.Kill();
                    CurrentProcess = proc;
                    exeIsHidden = true;
                    break;
                }

                if (!exeIsHidden)
                {
                    await WaitForExtension(p);

                    CurrentProcess = p;
                    BackgroundProcesses.Add((p, id));

                    if (p != null) _appWin = p.MainWindowHandle;

                    await Task.Delay(100);

                    MessageHelper.SendDataMessage(p, "set Name \"" + Config.Informations.Extension.Name + "\"");
                    MessageHelper.SendDataMessage(p, "set Color \"" + Config.Informations.Extension.Color + "\"");

                    SettingsHelper.SendSettingsBroadcast();

                    if (!string.IsNullOrEmpty(ExtensionsManager.FileToOpen))
                        MessageHelper.SendDataMessage(p, "open File \"" + ExtensionsManager.FileToOpen + "\"");
                }

                SetParent(_appWin, _panel.Handle);

                Index.Visibility = Visibility.Collapsed;
                IndexGrid.Visibility = Visibility.Visible;

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

                CurrentProcess = null;
            }

            Pages.Menu.ListViewMenu.IsEnabled = true;

            LoadingExe = false;
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

        private async Task RunWipeAnimation()
        {
            var opacity = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(opacity, new PropertyPath(OpacityProperty));

            var sb = new Storyboard();
            Storyboard.SetTarget(sb, _wipe);

            sb.Children.Add(opacity);

            sb.Begin();

            await Task.Delay(300);

            _wipe.Visibility = Visibility.Collapsed;

            sb.Stop();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyData = KeyInterop.VirtualKeyFromKey(e.Key);

            MessageHelper.SendDataMessage(CurrentProcess, "key \"" + keyData + "\"");

            e.Handled = true;
        }
    }
}