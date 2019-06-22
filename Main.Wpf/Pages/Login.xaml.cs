using Main.Wpf.Utilities;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Main.Wpf.Pages
{
    internal partial class Login
    {
        private readonly Ellipse Wipe = new Ellipse();

        public Login()
        {
            InitializeComponent();

            var palette = new PaletteHelper().QueryPalette();
            var hue = palette.PrimarySwatch.PrimaryHues.ToArray()[palette.PrimaryDarkHueIndex];

            Wipe.Fill = new SolidColorBrush(hue.Color);
            Wipe.VerticalAlignment = VerticalAlignment.Center;
            Wipe.HorizontalAlignment = HorizontalAlignment.Center;
            Wipe.Visibility = Visibility.Collapsed;
            System.Windows.Controls.Panel.SetZIndex(Wipe, 10);

            MainGrid.Children.Add(Wipe);
        }

        private void ShowInfoBox(string type = "", string text = "", int height = 0)
        {
            const int margin1 = 375;
            const int margin2 = 255;

            var converter = new BrushConverter();

            switch (type)
            {
                case "error":
                    InfoBox.Background = (Brush)converter.ConvertFromString("#B00020");
                    InfoBoxIcon.Kind = PackIconKind.ErrorOutline;
                    break;

                case "warning":
                    InfoBox.Background = (Brush)converter.ConvertFromString("#FF8800");
                    InfoBoxIcon.Kind = PackIconKind.WarningOutline;
                    break;

                case "success":
                    InfoBox.Background = (Brush)converter.ConvertFromString("#007E33");
                    InfoBoxIcon.Kind = PackIconKind.CheckOutline;
                    break;

                case "info":
                    InfoBox.Background = (Brush)converter.ConvertFromString("#0099CC");
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

            if (await Config.Login.FirstRun.Get())
            {
                return;
            }

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

            List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();

            for (var site = 0; site != Config.Menu.Sites.Count - 1; ++site)
            {
                sites.Add((Config.Menu.Sites[site].Title, Config.Menu.Sites[site].Icon, Config.Menu.Sites[site].Path, Config.Menu.Sites[site].StartArguments));
            }

            sites.Add(("Abmelden", "", "account.exe", ""));

            Config.Menu.Sites = sites;

            await mw.LoadExtensionAsync(true);

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

            Wipe.Visibility = Visibility.Visible;

            DoubleAnimation WipeHeight = new DoubleAnimation
            {
                From = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(WipeHeight, new PropertyPath(HeightProperty));

            DoubleAnimation WipeWidth = new DoubleAnimation
            {
                From = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(WipeWidth, new PropertyPath(WidthProperty));

            if (mw.ActualHeight > mw.ActualWidth)
            {
                WipeHeight.To = mw.ActualHeight * 2;
                WipeWidth.To = mw.ActualHeight * 2;
            }
            else
            {
                WipeHeight.To = mw.ActualWidth * 2;
                WipeWidth.To = mw.ActualWidth * 2;
            }

            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(sb, Wipe);

            sb.Children.Add(WipeHeight);
            sb.Children.Add(WipeWidth);

            sb.Begin();

            await Task.Delay(500);
        }
    }
}