using Main.Wpf.Functions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

            if (Config.ExtensionDirectoryName?.Length == 0)
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
                return;
            }

            AddonInstalledVersion.Text = Updater.Informations.Extension.Version.ToString();
            AddonVersion.Text = Updater.Informations.Extension.RunningVersion.ToString();
            AddonNewestVersion.Text = Updater.Informations.Extension.NewestVersion;

            ExtensionUpdateChannel.SelectedIndex = (int)Enum.Parse(typeof(Updater.UpdateChannels), Json.ReadString(Settings.Json, "updateChannel").ToLower());

            MainProgrammUpdateChannel.SelectedIndex = (int)Enum.Parse(typeof(Updater.UpdateChannels), Properties.Settings.Default.updateChannel.ToLower());

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btn.Click -= Button_Click;

            var FadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.4));
            var FadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
            btn_icon.BeginAnimation(OpacityProperty, FadeOut);
            btn_load.BeginAnimation(OpacityProperty, FadeIn);

            await Task.Run(() => Updater.Update(false));
            if (Config.ExtensionDirectoryName != "") await Task.Run(() => Updater.Update(true));

            MainProgrammVersion.Text = Updater.Informations.Programm.Version.ToString();
            MainProgrammNewestVersion.Text = Updater.Informations.Programm.NewestVersion;

            if (Config.ExtensionDirectoryName?.Length == 0)
            {
                ExtensionUpdateChannel.IsEnabled = false;
                ExtensionUpdateChannel.Text = "-";
                return;
            }

            AddonInstalledVersion.Text = Updater.Informations.Extension.Version.ToString();
            AddonVersion.Text = Updater.Informations.Extension.RunningVersion.ToString();
            AddonNewestVersion.Text = Updater.Informations.Extension.NewestVersion;

            ExtensionUpdateChannel.SelectedValue = Json.ReadString(Settings.Json, "updateChannel");

            MainProgrammUpdateChannel.SelectedValue = Properties.Settings.Default.updateChannel;

            btn_load.BeginAnimation(OpacityProperty, FadeOut);
            btn_icon.BeginAnimation(OpacityProperty, FadeIn);

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