using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Main.Wpf.Utilities;
using MaterialDesignThemes.Wpf;

namespace Main.Wpf.Pages
{
    internal partial class Login
    {
        private readonly Ellipse _wipe = new Ellipse();

        [Obsolete]
        public Login()
        {
            InitializeComponent();

            var palette = new PaletteHelper().QueryPalette();
            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];

            _wipe.Fill = new SolidColorBrush(hue.Color);
            _wipe.VerticalAlignment = VerticalAlignment.Center;
            _wipe.HorizontalAlignment = HorizontalAlignment.Center;
            _wipe.Visibility = Visibility.Collapsed;
            Panel.SetZIndex(_wipe, 10);

            MainGrid.Children.Add(_wipe);
        }

        private void ShowInfoBox(string type = "", string text = "", int height = 0)
        {
            const int margin1 = 375;
            const int margin2 = 255;

            var converter = new BrushConverter();

            switch (type)
            {
                case "error":
                    InfoBox.Background = (Brush) converter.ConvertFromString("#B00020");
                    InfoBoxIcon.Kind = PackIconKind.ErrorOutline;
                    break;

                case "warning":
                    InfoBox.Background = (Brush) converter.ConvertFromString("#FF8800");
                    InfoBoxIcon.Kind = PackIconKind.WarningOutline;
                    break;

                case "success":
                    InfoBox.Background = (Brush) converter.ConvertFromString("#007E33");
                    InfoBoxIcon.Kind = PackIconKind.CheckOutline;
                    break;

                case "info":
                    InfoBox.Background = (Brush) converter.ConvertFromString("#0099CC");
                    InfoBoxIcon.Kind = PackIconKind.InfoOutline;
                    break;

                default:
                {
                    if (InfoBox.Margin != new Thickness(0, 0, 0, margin1)) return;

                    var taClose = new ThicknessAnimation(InfoBox.Margin, new Thickness(0, 0, 0, margin2),
                        TimeSpan.FromSeconds(0.4));
                    InfoBox.BeginAnimation(MarginProperty, taClose);

                    return;
                }
            }

            InfoBoxText.Text = text;

            var ta = new ThicknessAnimation(new Thickness(0, 0, 0, margin2), new Thickness(0, 0, 0, margin1 + height),
                TimeSpan.FromSeconds(0.4));
            InfoBox.BeginAnimation(MarginProperty, ta);
        }

        private void InfoBoxClose_OnClick(object sender, RoutedEventArgs e)
        {
            ShowInfoBox();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            mw.Title = Title + " - " + Config.Informations.Extension.Name;

            Welcome.Text = "Willkommen bei " + Environment.NewLine + Config.Informations.Extension.Name + "!";

            try
            {
                if (!string.IsNullOrEmpty(Config.Informations.Extension.Favicon))
                    Logo.Source = new BitmapImage(new Uri(Config.Informations.Extension.Favicon));
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            if (await Config.Login.FirstRun.Get()) return;

            ShowInfoBox("info", "Der Anmelde-Prozess läuft gerade ...");

            IsEnabled = false;

            if (!InternetHelper.CheckConnection())
            {
                ShowInfoBox();

                await mw.LoadExtensionAsync();

                IsEnabled = true;
            }
            else
            {
                try
                {
                    var loginResult = await AccountHelper.Client.LoginAsync();

                    AccountHelper.UserId = loginResult.User.FindFirst(c => c.Type == "sub")?.Value;

                    if (loginResult.IsError)
                    {
                        LogFile.WriteLog(loginResult.Error);
                        ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                        IsEnabled = true;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogFile.WriteLog(ex);
                    ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                    IsEnabled = true;
                    return;
                }

                IsEnabled = true;
                ShowInfoBox();

                await RunWipeAnimation();

                await Config.Login.LoggedIn.Set(true);
                await Config.Login.FirstRun.Set(false);

                await mw.LoadExtensionAsync(true);

                while (!ConfigHelper._loaded) await Task.Delay(100);

                var sites = Config.Menu.Sites;

                foreach (var item in sites)
                    if (item.Path == "account.exe" && item.Title == "Anmelden")
                    {
                        item.Title = "Abmelden";
                        item.Icon = PackIconKind.AccountMinusOutline;
                        break;
                    }

                await Config.Menu.SetSites(sites);

                IsEnabled = true;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            ShowInfoBox();

            if (!InternetHelper.CheckConnection())
            {
                ShowInfoBox("warning", "Bitte überprüfe deine Internet-Verbindung!");
                return;
            }

            ShowInfoBox("info", "Der Anmelde-Prozess läuft gerade ...");
            IsEnabled = false;

            try
            {
                var loginResult = await AccountHelper.Client.LoginAsync();

                AccountHelper.UserId = loginResult.User.FindFirst(c => c.Type == "sub")?.Value;

                if (loginResult.IsError)
                {
                    LogFile.WriteLog(loginResult.Error);
                    ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                    IsEnabled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
                ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                IsEnabled = true;
                return;
            }

            IsEnabled = true;

            ShowInfoBox();

            await RunWipeAnimation();

            await Config.Login.LoggedIn.Set(true);
            await Config.Login.FirstRun.Set(false);

            await mw.LoadExtensionAsync(true);

            while (!ConfigHelper._loaded) await Task.Delay(100);

            var sites = Config.Menu.Sites;

            foreach (var item in sites)
                if (item.Path == "account.exe" && item.Title == "Anmelden")
                {
                    item.Title = "Abmelden";
                    item.Icon = PackIconKind.AccountMinusOutline;
                    break;
                }

            await Config.Menu.SetSites(sites);

            IsEnabled = true;
        }

        private async void Skip_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            await RunWipeAnimation();

            await Config.Login.LoggedIn.Set(false);
            await Config.Login.FirstRun.Set(false);

            ShowInfoBox();

            await mw.LoadExtensionAsync(true);

            IsEnabled = true;
        }

        private async Task RunWipeAnimation()
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            _wipe.Visibility = Visibility.Visible;

            var wipeHeight = new DoubleAnimation
            {
                From = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(wipeHeight, new PropertyPath(HeightProperty));

            var wipeWidth = new DoubleAnimation
            {
                From = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(wipeWidth, new PropertyPath(WidthProperty));

            if (mw.ActualHeight > mw.ActualWidth)
            {
                wipeHeight.To = mw.ActualHeight * 2;
                wipeWidth.To = mw.ActualHeight * 2;
            }
            else
            {
                wipeHeight.To = mw.ActualWidth * 2;
                wipeWidth.To = mw.ActualWidth * 2;
            }

            var sb = new Storyboard();
            Storyboard.SetTarget(sb, _wipe);

            sb.Children.Add(wipeHeight);
            sb.Children.Add(wipeWidth);

            sb.Begin();

            await Task.Delay(500);
        }
    }
}