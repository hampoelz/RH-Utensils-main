using Main.Wpf.Functions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf
{
    public partial class App : Application
    {
        public static string[] Parameters;

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            Parameters = e.Args;

            await Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () => await Versioning.Start().ConfigureAwait(false)));

            await Task.Run(Updater.BackgroundProgrammUpdate);

            Window window = new MainWindow();
            window.Show();

            await Task.Run(() => Updater.Update(false));
            if (Config.ExtensionDirectoryName != "") await Task.Run(() => Updater.Update(true));
        }
    }
}