using System.Diagnostics;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class InstanceHelper
    {
        public static Process GetMainProcess()
        {
            Process[] _allProcs = Process.GetProcessesByName("RH Utensils");

            for (int i = 0; i < _allProcs.Length; i++)
            {
                if (_allProcs[i].MainWindowTitle.EndsWith(Config.Name))
                {
                    return _allProcs[i];
                }
            }

            return null;
        }
    }
}