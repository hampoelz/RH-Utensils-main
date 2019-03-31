using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Main.Wpf.Pages
{
    internal partial class Selector
    {
        public Selector()
        {
            InitializeComponent();
        }

        private void ShowInfoBox(string type = "", string text = "", int height = 0)
        {
            const int margin1 = 260;
            const int margin2 = 140;

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

        private void Extension_DropDownOpened(object sender, EventArgs e)
        {
            var extensionsDirectory = App.ExtensionsDirectory;

            var extensionsDirectories = Directory.GetDirectories(extensionsDirectory);
            var extensions = Directory.GetDirectories(extensionsDirectory);

            for (var i = 0; i != extensionsDirectories.Length; ++i)
            {
                extensions[i] = extensions[i].Replace(extensionsDirectory, "").Replace(@"\", "").Replace("/", "");

                var installedVersions = Directory.GetDirectories(extensionsDirectories[i]);

                for (var ii = 0; ii != installedVersions.Length; ++ii)
                {
                    installedVersions[ii] = installedVersions[ii].Replace(extensionsDirectories[i], "")
                        .Replace(@"\", "").Replace("/", "");

                    try
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new Version(installedVersions[ii]);
                    }
                    catch
                    {
                        installedVersions = installedVersions.Where(val => val != installedVersions[ii]).ToArray();
                        --ii;
                    }
                }

                if (installedVersions.Length != 0) continue;

                extensions = extensions.Where(val => val != extensions[i]).ToArray();
                extensionsDirectories = extensionsDirectories.Where(val => val != extensionsDirectories[i]).ToArray();
                --i;
            }

            Extension.ItemsSource = extensions;

            ShowInfoBox();
        }

        private void Version_DropDownOpened(object sender, EventArgs e)
        {
            var extensionsDirectory = App.ExtensionsDirectory;

            var extensionsDirectories = Directory.GetDirectories(extensionsDirectory);
            var extensions = Directory.GetDirectories(extensionsDirectory);

            for (var i = 0; i != extensionsDirectories.Length; ++i)
            {
                extensions[i] = extensions[i].Replace(extensionsDirectory, "").Replace(@"\", "").Replace("/", "");

                var installedVersions = Directory.GetDirectories(extensionsDirectories[i]);

                for (var ii = 0; ii != installedVersions.Length; ++ii)
                {
                    installedVersions[ii] = installedVersions[ii].Replace(extensionsDirectories[i], "")
                        .Replace(@"\", "").Replace("/", "");

                    try
                    {
                        // ReSharper disable once ObjectCreationAsStatement
                        new Version(installedVersions[ii]);
                    }
                    catch
                    {
                        installedVersions = installedVersions.Where(val => val != installedVersions[ii]).ToArray();
                        --ii;
                    }
                }

                if (Extension.Text == extensions[i])
                    Version.ItemsSource = installedVersions;
            }

            ShowInfoBox();
        }

        private void Extension_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Version.IsEnabled = true;

            Version.ItemsSource = "";
            Start.IsEnabled = false;
            Uninstall.IsEnabled = false;
        }

        private void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Uninstall.IsEnabled = true;
            Start.IsEnabled = true;
        }

        private void Uninstall_OnClick(object sender, RoutedEventArgs e)
        {
            ShowInfoBox("warning", "Willst du diese Add-on Version wirklich Deinstallieren?");
            ConfirmUninstall.Visibility = Visibility.Visible;
        }

        private void ConfirmUninstall_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var extensionsDirectory = App.ExtensionsDirectory + @"\" + Extension.Text + @"\" + Version.Text;

                Directory.Delete(extensionsDirectory, true);

                ShowInfoBox("success", "Das Add-on wurde erfolgreich deinstalliert.");
                ConfirmUninstall.Visibility = Visibility.Collapsed;
                Version.ItemsSource = "";
                Start.IsEnabled = false;
                Uninstall.IsEnabled = false;
            }
            catch
            {
                ShowInfoBox("error", "Beim Deinstallieren des Add-ons ist ein Fehler aufgetreten!");
                ConfirmUninstall.Visibility = Visibility.Collapsed;
            }
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var ps = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
                {
                    Arguments = "-" + Extension.Text + " -version " + Version.Text
                };
                Process.Start(ps);

                Application.Current.Shutdown();
            }
            catch
            {
                ShowInfoBox("error", "Beim Starten des Add-ons ist ein Fehler aufgetreten!");
            }
        }
    }
}