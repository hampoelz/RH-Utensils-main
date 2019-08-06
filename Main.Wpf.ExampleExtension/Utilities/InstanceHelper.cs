using System.Diagnostics;
using System.Linq;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class InstanceHelper
    {
        public static Process GetMainProcess()
        {
            var allProcs = Process.GetProcessesByName("RH Utensils");

            return allProcs.FirstOrDefault(t => t.MainWindowTitle.EndsWith(Config.Name));
        }
    }
}