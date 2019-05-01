using Main.Wpf.Functions;
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
        private bool _loaded;

        public About()
        {
            InitializeComponent();

            Title = "Über " + Informations.Extension.Name;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Informations.Extension.Favicon != "")
                    Image.Source = new BitmapImage(new Uri(Informations.Extension.Favicon));
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            foreach (var channel in Enum.GetValues(typeof(Updater.UpdateChannels)))
            {
                ExtensionUpdateChannel.Items.Add(channel);
                MainProgrammUpdateChannel.Items.Add(channel);
            }

            Extension.Text = Informations.Extension.Name;
            Extension1.Text = Informations.Extension.Name;
            Extension2.Text = Informations.Extension.Name;
            Extension3.Text = Informations.Extension.Name;
            Developer.Text = Informations.Developer.Organisation;
            Copyright.Text = Informations.Copyright.Organisation;

            MainProgrammVersion.Text = Updater.Informations.Programm.Version.ToString();
            MainProgrammNewestVersion.Text = Updater.Informations.Programm.NewestVersion;

            if (Config.ExtensionDirectoryName != "")
            {
                AddOn.Text = Informations.Extension.Name;
                AddonInstalledVersion.Text = Updater.Informations.Extension.Version.ToString();
                AddonVersion.Text = Updater.Informations.Extension.RunningVersion.ToString();
                AddonNewestVersion.Text = Updater.Informations.Extension.NewestVersion;

                ExtensionUpdateChannel.SelectedIndex = (int)Enum.Parse(typeof(Updater.UpdateChannels), Json.ReadString(Settings.Json, "updateChannel").ToLower());
            }
            else
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
            }

            MainProgrammUpdateChannel.SelectedIndex = (int)Enum.Parse(typeof(Updater.UpdateChannels), Properties.Settings.Default.updateChannel.ToLower());

            isDownloading();

            _loaded = true;
        }

        private void Extension_Website_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Informations.Extension.Website);
        }

        private void Developer_Website_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Informations.Developer.Website);
        }

        private void MainProgramm_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/rh-utensils/main");
        }

        private void RHUtensils_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://rh-utensils.hampoelz.net/");
        }

        private void Extension_SourceCode_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Informations.Extension.SourceCode);
        }

        private void Extension_Copyright_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Informations.Copyright.Website);
        }

        private void HampisProjekte_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://hampoelz.net/");
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            MainWindow.IsAbout = false;
        }

        private bool _isDownloading;

        private async void isDownloading()
        {
            if (_isDownloading) return;

            if (!Updater.isDownloading) return;

            _isDownloading = true;

            btn.IsEnabled = false;

            var Expand = new ThicknessAnimation(new Thickness(10, 0, 10, 5), TimeSpan.FromMilliseconds(250));
            var Collapse = new ThicknessAnimation(new Thickness(10, 0, 310, 5), TimeSpan.FromMilliseconds(250));
            var FadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            var FadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));

            InstallUpdateInfo.BeginAnimation(OpacityProperty, FadeOut);
            DownloadUpdateInfo.BeginAnimation(OpacityProperty, FadeIn);

            await Task.Delay(350);

            InfoCard.BeginAnimation(MarginProperty, Expand);

            while (Updater.isDownloading)
            {
                await Task.Delay(1000);
            }

            InfoCard.BeginAnimation(MarginProperty, Collapse);

            await Task.Delay(200);

            InstallUpdateInfo.BeginAnimation(OpacityProperty, FadeIn);
            DownloadUpdateInfo.BeginAnimation(OpacityProperty, FadeOut);

            _isDownloading = false;

            btn.IsEnabled = true;
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btn.Click -= Button_Click;

            var timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Normal, delegate
            {
                isDownloading();
            }, Application.Current.Dispatcher);

            var FadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            var FadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
            btn_icon.BeginAnimation(OpacityProperty, FadeOut);
            btn_load.BeginAnimation(OpacityProperty, FadeIn);

            await Task.Run(() => Updater.Update(false));

            MainProgrammVersion.Text = Updater.Informations.Programm.Version.ToString();
            MainProgrammNewestVersion.Text = Updater.Informations.Programm.NewestVersion;

            if (Config.ExtensionDirectoryName != "")
            {
                await Task.Run(() => Updater.Update(true));

                AddonInstalledVersion.Text = Updater.Informations.Extension.Version.ToString();
                AddonVersion.Text = Updater.Informations.Extension.RunningVersion.ToString();
                AddonNewestVersion.Text = Updater.Informations.Extension.NewestVersion;

                ExtensionUpdateChannel.SelectedIndex = (int)Enum.Parse(typeof(Updater.UpdateChannels), Json.ReadString(Settings.Json, "updateChannel").ToLower());
            }
            else
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
            }

            MainProgrammUpdateChannel.SelectedIndex = (int)Enum.Parse(typeof(Updater.UpdateChannels), Properties.Settings.Default.updateChannel.ToLower());

            btn_load.BeginAnimation(OpacityProperty, FadeOut);
            btn_icon.BeginAnimation(OpacityProperty, FadeIn);

            timer.Stop();

            btn.Click += Button_Click;
        }

        private void MainProgrammUpdateChannel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_loaded) return;

            if (Properties.Settings.Default.updateChannel != MainProgrammUpdateChannel.SelectedItem.ToString())
            {
                Properties.Settings.Default.updateChannel = MainProgrammUpdateChannel.SelectedItem.ToString();
                Properties.Settings.Default.Save();
            }
        }

        private void ExtensionUpdateChannel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_loaded) return;

            if (Json.ReadString(Settings.Json, "updateChannel") != ExtensionUpdateChannel.SelectedItem.ToString())
                Settings.Json = Json.ChangeValue(Settings.Json, "updateChannel", ExtensionUpdateChannel.SelectedItem.ToString());
        }
    }
}