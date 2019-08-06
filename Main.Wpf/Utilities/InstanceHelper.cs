using System.Diagnostics;
using System.Linq;

namespace Main.Wpf.Utilities
{
    public static class InstanceHelper
    {
        public static bool CheckInstances()
        {
            if (Config.Informations.Extension.Name == "RH Utensils") return false;

            var currentProcess = Process.GetCurrentProcess();
            var allProcesses = Process.GetProcessesByName(currentProcess.ProcessName);

            return allProcesses.Length > 1 && allProcesses.Any(process =>
                       process.MainWindowTitle.EndsWith(Config.Informations.Extension.Name));
        }

        public static Process GetAlreadyRunningInstance()
        {
            var currentProc = Process.GetCurrentProcess();
            var allProcs = Process.GetProcessesByName(currentProc.ProcessName);

            return allProcs.Where(t => t.Id != currentProc.Id)
                .FirstOrDefault(t => t.MainWindowTitle.EndsWith(Config.Informations.Extension.Name));
        }
    }
}