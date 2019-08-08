using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Main.Wpf.Utilities;

namespace Main.Wpf
{
    public partial class App
    {
        public static string[] Parameters;

        private Mutex _mutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [Obsolete]
        private async void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Parameters = e.Args;

                if (Current.Dispatcher != null)
                    await Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(async () => await ExtensionsManager.LoadExtension()));

                Mutex(Config.Informations.Extension.Name);

                await Task.Run(UpdateHelper.BackgroundProgrammUpdate);

                Window window = new MainWindow();
                window.Show();

                await Task.Run(() => UpdateHelper.Update(false));
                if (!string.IsNullOrEmpty(Config.ExtensionDirectoryName))
                    await Task.Run(() => UpdateHelper.Update(true));

                if (!string.IsNullOrEmpty(Config.ExtensionDirectoryName)) SettingsHelper.CreateSettingsWatcher();

                LogFile.DeleteOldLogFiles();
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
                Current.Shutdown();
            }
        }

        private void Mutex(string name)
        {
            _mutex = new Mutex(true, name, out var createdNew);

            if (!createdNew)
            {
                var current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id == current.Id) continue;

                    SetForegroundWindow(process.MainWindowHandle);

                    MessageHelper.SendDataMessage(process, "open File \"" + ExtensionsManager.FileToOpen + "\"");

                    break;
                }

                Current.Shutdown();
            }
            else
            {
                Exit += CloseMutexHandler;
            }
        }

        protected virtual void CloseMutexHandler(object sender, EventArgs e)
        {
            _mutex?.Close();
        }
    }
}