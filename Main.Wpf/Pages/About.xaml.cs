using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Main.Wpf.Pages
{
    internal partial class About
    {
        public About()
        {
            InitializeComponent();

            Title = "Über " + App.Name;

            if (App.Favicon != "") Image.Source = new BitmapImage(new Uri(App.Favicon));
        }

        public static DispatcherTimer _timer = new DispatcherTimer();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainProgrammVersion.Text = App.ProgrammVersion.ToString();
            MainProgrammNewestVersion.Text = App.ProgrammUpdateVersion;

            if (App.ExtensionName != "") AddonInstalledVersion.Text = App.ExtensionMaxVersion.ToString();
            if (App.ExtensionName != "") AddonVersion.Text = App.ExtensionVersion.ToString();
            AddonNewestVersion.Text = App.ExtensionUpdateVersion;

            if (App.ExtensionName != "")
            {
                ExtensionUpdateChannel.SelectedValue = Functions.Json.ConvertToString(App.SettingsJson, "updateChannel");
            }
            else
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
            }
            MainProgrammUpdateChannel.SelectedValue = Properties.Settings.Default.updateChannel;

            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Functions.Json.ConvertToString(App.SettingsJson, "updateChannel") != ExtensionUpdateChannel.Text)
                Functions.Settings.Set(Functions.Json.ChangeValue(App.SettingsJson, "updateChannel", ExtensionUpdateChannel.Text));

            if (Properties.Settings.Default.updateChannel != MainProgrammUpdateChannel.Text)
            {
                Properties.Settings.Default.updateChannel = MainProgrammUpdateChannel.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void Website_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(App.Website);
        }

        private void DeveloperWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(App.DeveloperWebsite);
        }

        private void MainProgramm_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/rh-utensils/main");
        }

        private void RHUtensils_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://rh-utensils.hampoelz.net/");
        }

        private void SourceCode_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(App.SourceCode);
        }

        private void Copyright_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(App.CopyrightWebsite);
        }

        private void HampisProjekte_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://hampoelz.net/");
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            MainWindow.IsAbout = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btn.Click -= Button_Click;

            var FadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.4));
            var FadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
            btn_icon.BeginAnimation(OpacityProperty, FadeOut);
            btn_load.BeginAnimation(OpacityProperty, FadeIn);

            await Task.Run(() => Functions.Updater.Update(false));
            if (App.ExtensionName != "") await Task.Run(() => Functions.Updater.Update(true));

            MainProgrammVersion.Text = App.ProgrammVersion.ToString();
            MainProgrammNewestVersion.Text = App.ProgrammUpdateVersion;

            if (App.ExtensionName != "") AddonInstalledVersion.Text = App.ExtensionMaxVersion.ToString();
            if (App.ExtensionName != "") AddonVersion.Text = App.ExtensionVersion.ToString();
            AddonNewestVersion.Text = App.ExtensionUpdateVersion;

            if (App.ExtensionName != "")
                ExtensionUpdateChannel.SelectedValue = Functions.Json.ConvertToString(App.SettingsJson, "updateChannel");
            else
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
            }
            MainProgrammUpdateChannel.SelectedValue = Properties.Settings.Default.updateChannel;

            btn_load.BeginAnimation(OpacityProperty, FadeOut);
            btn_icon.BeginAnimation(OpacityProperty, FadeIn);

            btn.Click += Button_Click;
        }
    }
}