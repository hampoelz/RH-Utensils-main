using Main.Wpf.Properties;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Main.Wpf.Pages
{
    internal partial class Login
    {
        public Login()
        {
            InitializeComponent();
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

            mw.Title = Title + " - " + App.Name;

            Welcome.Text = "Willkommen bei " + Environment.NewLine + App.Name + "!";

            if (App.Favicon != "")
                Logo.Source = new BitmapImage(new Uri(App.Favicon));

            if (!App.ShowLogin && (!Settings.Default.login || App.ShowFirstPage || App.SkipLogin)) return;

            ShowInfoBox("info", "Der Anmelde-Prozess läuft gerade ...");

            IsEnabled = false;

            if (!Functions.InternetChecker.Check())
            {
                ShowInfoBox();

                await mw.LoadExtensionAsync();

                IsEnabled = true;
            }
            else
            {
                try
                {
                    var loginResult = await Functions.Account.Client.LoginAsync();

                    Functions.Account.UserId = loginResult.User.FindFirst(c => c.Type == "sub")?.Value;

                    if (loginResult.IsError)
                    {
                        ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                        IsEnabled = true;
                        return;
                    }
                }
                catch
                {
                    ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                    IsEnabled = true;
                    return;
                }

                IsEnabled = true;
                ShowInfoBox();

                Settings.Default.login = true;
                Settings.Default.skipLogin = false;
                Settings.Default.Save();

                if (App.ExtensionName != "") Functions.Settings.StartSync();

                await mw.LoadExtensionAsync();

                IsEnabled = true;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            ShowInfoBox();

            if (!Functions.InternetChecker.Check())
            {
                ShowInfoBox("warning", "Bitte überprüfe deine Internet-Verbindung!");
                return;
            }

            ShowInfoBox("info", "Der Anmelde-Prozess läuft gerade ...");
            IsEnabled = false;

            try
            {
                var loginResult = await Functions.Account.Client.LoginAsync();

                Functions.Account.UserId = loginResult.User.FindFirst(c => c.Type == "sub")?.Value;

                if (loginResult.IsError)
                {
                    ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                    IsEnabled = true;
                    return;
                }
            }
            catch
            {
                ShowInfoBox("error", "Beim Anmelden ist leider ein Fehler aufgetreten!");
                IsEnabled = true;
                return;
            }

            IsEnabled = true;

            ShowInfoBox();

            Settings.Default.login = true;
            Settings.Default.skipLogin = false;
            Settings.Default.Save();

            if (App.ExtensionName != "") Functions.Settings.StartSync();

            await mw.LoadExtensionAsync();

            IsEnabled = true;
        }

        private async void Skip_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            Settings.Default.login = false;
            Settings.Default.skipLogin = true;
            Settings.Default.Save();

            ShowInfoBox();

            await mw.LoadExtensionAsync();

            IsEnabled = true;
        }
    }
}