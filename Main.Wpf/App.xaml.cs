using Main.Wpf.Utilities;
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

            await Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () => await ExtensionsManager.LoadExtension().ConfigureAwait(false)));

            if (InstanceHelper.CheckInstances())
            {
                MessageHelper.SendDataMessage(InstanceHelper.GetAlreadyRunningInstance(), "It Works!!!");
                Application.Current.Shutdown();
            }

            await Task.Run(UpdateHelper.BackgroundProgrammUpdate);

            Window window = new MainWindow();
            window.Show();

            await Task.Run(() => UpdateHelper.Update(false));
            if (Config.ExtensionDirectoryName != "") await Task.Run(() => UpdateHelper.Update(true));
        }
    }
}