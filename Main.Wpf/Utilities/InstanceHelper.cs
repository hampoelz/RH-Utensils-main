using System.Diagnostics;

namespace Main.Wpf.Utilities
{
    public static class InstanceHelper
    {
        public static bool CheckInstances()
        {
            if (Config.Informations.Extension.Name == "RH Utensils") return false;

            Process _currentProcess = Process.GetCurrentProcess();
            Process[] _allProcesses = Process.GetProcessesByName(_currentProcess.ProcessName);

            if (_allProcesses.Length > 1)
            {
                foreach (var process in _allProcesses)
                {
                    if (process.MainWindowTitle.EndsWith(Config.Informations.Extension.Name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Process GetAlreadyRunningInstance()
        {
            Process _currentProc = Process.GetCurrentProcess();
            Process[] _allProcs = Process.GetProcessesByName(_currentProc.ProcessName);

            for (int i = 0; i < _allProcs.Length; i++)
            {
                if (_allProcs[i].Id != _currentProc.Id)
                {
                    if (_allProcs[i].MainWindowTitle.EndsWith(Config.Informations.Extension.Name))
                    {
                        return _allProcs[i];
                    }
                }
            }

            return null;
        }
    }
}