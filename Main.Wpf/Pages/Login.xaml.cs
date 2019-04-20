using Main.Wpf.Functions;
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

            mw.Title = Title + " - " + Informations.Extension.Name;

            Welcome.Text = "Willkommen bei " + Environment.NewLine + Informations.Extension.Name + "!";

            try
            {
                if (Informations.Extension.Favicon != "")
                    Logo.Source = new BitmapImage(new Uri(Informations.Extension.Favicon));
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            if (await Functions.Login.FirstRun.Get())
            {
                return;
            }

            ShowInfoBox("info", "Der Anmelde-Prozess läuft gerade ...");

            IsEnabled = false;

            if (!InternetChecker.Check())
            {
                ShowInfoBox();

                await mw.LoadExtensionAsync();

                IsEnabled = true;
            }
            else
            {
                try
                {
                    var loginResult = await Account.Client.LoginAsync();

                    Account.UserId = loginResult.User.FindFirst(c => c.Type == "sub")?.Value;

                    if (loginResult.IsError)
                    {
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

                await Functions.Login.LoggedIn.Set(true);
                await Functions.Login.FirstRun.Set(false);

                Settings.StartSync();

                await mw.LoadExtensionAsync(true);

                IsEnabled = true;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            ShowInfoBox();

            if (!InternetChecker.Check())
            {
                ShowInfoBox("warning", "Bitte überprüfe deine Internet-Verbindung!");
                return;
            }

            ShowInfoBox("info", "Der Anmelde-Prozess läuft gerade ...");
            IsEnabled = false;

            try
            {
                var loginResult = await Account.Client.LoginAsync();

                Account.UserId = loginResult.User.FindFirst(c => c.Type == "sub")?.Value;

                if (loginResult.IsError)
                {
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

            await Functions.Login.LoggedIn.Set(true);
            await Functions.Login.FirstRun.Set(false);

            List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();

            for (var site = 0; site != Functions.Menu.Sites.Count - 1; ++site)
            {
                sites.Add((Functions.Menu.Sites[site].Title, Functions.Menu.Sites[site].Icon, Functions.Menu.Sites[site].Path, Functions.Menu.Sites[site].StartArguments));
            }

            sites.Add(("Abmelden", "", "account.exe", ""));

            Functions.Menu.Sites = sites;

            Settings.StartSync();

            await mw.LoadExtensionAsync(true);

            IsEnabled = true;
        }

        private async void Skip_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            await RunWipeAnimation();

            await Functions.Login.LoggedIn.Set(false);
            await Functions.Login.FirstRun.Set(false);

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