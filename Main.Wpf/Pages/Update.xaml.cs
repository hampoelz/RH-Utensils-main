using Main.Wpf.Functions;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Main.Wpf.Pages
{
    internal partial class Update
    {
        public Update()
        {
            InitializeComponent();
        }

        private async void BtnSkip_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            await mw.Login();
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(Path.Combine(Config.ExtensionsDirectory, Informations.Extension.Name, "updater.exe")))
                {
                    var ps = new ProcessStartInfo(Path.Combine(Config.ExtensionsDirectory, Informations.Extension.Name, "updater.exe"));
                    LogFile.WriteLog("Setup wird gestartet.");
                    Process.Start(ps);

                    Application.Current.Shutdown();
                }

                if (File.Exists(@".\updater.exe"))
                {
                    var ps = new ProcessStartInfo(@".\updater.exe");
                    LogFile.WriteLog("Setup wird gestartet.");
                    Process.Start(ps);

                    Application.Current.Shutdown();
                }

                LogFile.WriteLog("Kein Setup gefunden ...");

                if (!(Application.Current.MainWindow is MainWindow mw)) return;

                await mw.Login();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }
    }
}