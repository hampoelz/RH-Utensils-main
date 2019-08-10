using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Main.Wpf.Properties;
using Main.Wpf.Utilities;

namespace Main.Wpf.Pages
{
    internal partial class About
    {
        private bool _isDownloading;
        private bool _loaded;

        public About()
        {
            InitializeComponent();

            Title = "Über " + Config.Informations.Extension.Name;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Config.Informations.Extension.Favicon))
                    Image.Source = new BitmapImage(new Uri(Config.Informations.Extension.Favicon));
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            foreach (var channel in Enum.GetValues(typeof(UpdateHelper.UpdateChannels)))
            {
                ExtensionUpdateChannel.Items.Add(channel);
                MainProgrammUpdateChannel.Items.Add(channel);
            }

            Extension.Text = Config.Informations.Extension.Name;
            Extension1.Text = Config.Informations.Extension.Name;
            Extension2.Text = Config.Informations.Extension.Name;
            Extension3.Text = Config.Informations.Extension.Name;
            Developer.Text = Config.Informations.Developer.Organisation;
            Copyright.Text = Config.Informations.Copyright.Organisation;

            MainProgrammVersion.Text = Config.Updater.Programm.Version.ToString();
            MainProgrammNewestVersion.Text = Config.Updater.Programm.NewestVersion;

            if (!string.IsNullOrEmpty(Config.ExtensionDirectoryName))
            {
                AddOn.Text = Config.Informations.Extension.Name;
                AddonInstalledVersion.Text = Config.Updater.Extension.Version.ToString();
                AddonVersion.Text = Config.Updater.Extension.RunningVersion.ToString();
                AddonNewestVersion.Text = Config.Updater.Extension.NewestVersion;

                ExtensionUpdateChannel.SelectedIndex = (int) Enum.Parse(typeof(UpdateHelper.UpdateChannels),
                    JsonHelper.ReadString(Config.Settings.Json, "updateChannel").ToLower());
            }
            else
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
            }

            MainProgrammUpdateChannel.SelectedIndex = (int) Enum.Parse(typeof(UpdateHelper.UpdateChannels),
                Config.Settings.MainUpdateChannel.ToLower());

            IsDownloading();

            _loaded = true;
        }

        private void Extension_Website_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.Informations.Extension.Website);
        }

        private void Developer_Website_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.Informations.Developer.Website);
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
            Process.Start(Config.Informations.Extension.SourceCode);
        }

        private void Extension_Copyright_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.Informations.Copyright.Website);
        }

        private void HampisProjekte_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://hampoelz.net/");
        }

        private async void IsDownloading()
        {
            if (_isDownloading) return;

            if (!UpdateHelper.IsDownloading) return;

            _isDownloading = true;

            CheckUpdatesButton.IsEnabled = false;

            var expand = new ThicknessAnimation(new Thickness(10, 0, 10, 5), TimeSpan.FromMilliseconds(250));
            var collapse = new ThicknessAnimation(new Thickness(10, 0, 310, 5), TimeSpan.FromMilliseconds(250));
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));

            InstallUpdateInfo.BeginAnimation(OpacityProperty, fadeOut);
            DownloadUpdateInfo.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(350);

            InfoCard.BeginAnimation(MarginProperty, expand);

            while (UpdateHelper.IsDownloading) await Task.Delay(1000);

            InfoCard.BeginAnimation(MarginProperty, collapse);

            await Task.Delay(200);

            InstallUpdateInfo.BeginAnimation(OpacityProperty, fadeIn);
            DownloadUpdateInfo.BeginAnimation(OpacityProperty, fadeOut);

            _isDownloading = false;

            CheckUpdatesButton.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckUpdatesButton.Click -= Button_Click;

            try
            {
                var timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Normal,
                    delegate { IsDownloading(); },
                    Application.Current.Dispatcher ?? throw new InvalidOperationException());

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
                CheckUpdatesIcon.BeginAnimation(OpacityProperty, fadeOut);
                CheckUpdatesProgressBar.BeginAnimation(OpacityProperty, fadeIn);

                await Task.Run(() => UpdateHelper.Update(false));

                MainProgrammVersion.Text = Config.Updater.Programm.Version.ToString();
                MainProgrammNewestVersion.Text = Config.Updater.Programm.NewestVersion;

                if (!string.IsNullOrEmpty(Config.ExtensionDirectoryName))
                {
                    await Task.Run(() => UpdateHelper.Update(true));

                    AddonInstalledVersion.Text = Config.Updater.Extension.Version.ToString();
                    AddonVersion.Text = Config.Updater.Extension.RunningVersion.ToString();
                    AddonNewestVersion.Text = Config.Updater.Extension.NewestVersion;

                    ExtensionUpdateChannel.SelectedIndex = (int) Enum.Parse(typeof(UpdateHelper.UpdateChannels),
                        JsonHelper.ReadString(Config.Settings.Json, "updateChannel").ToLower());
                }
                else
                {
                    ExtensionUpdateChannel.IsEnabled = false;
                    ExtensionUpdateChannel.Text = "-";
                }

                MainProgrammUpdateChannel.SelectedIndex = (int) Enum.Parse(typeof(UpdateHelper.UpdateChannels),
                    Config.Settings.MainUpdateChannel.ToLower());

                CheckUpdatesProgressBar.BeginAnimation(OpacityProperty, fadeOut);
                CheckUpdatesIcon.BeginAnimation(OpacityProperty, fadeIn);

                timer.Stop();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            CheckUpdatesButton.Click += Button_Click;
        }

        private void MainProgrammUpdateChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded) return;

            if (Config.Settings.MainUpdateChannel.ToLower() == MainProgrammUpdateChannel.SelectedItem.ToString()) return;

            Config.Settings.MainUpdateChannel = MainProgrammUpdateChannel.SelectedItem.ToString();
        }

        private void ExtensionUpdateChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded) return;

            if (JsonHelper.ReadString(Config.Settings.Json, "updateChannel") !=
                ExtensionUpdateChannel.SelectedItem.ToString())
                Config.Settings.Json = JsonHelper.ChangeValue(Config.Settings.Json, "updateChannel",
                    ExtensionUpdateChannel.SelectedItem.ToString());
        }
    }
}