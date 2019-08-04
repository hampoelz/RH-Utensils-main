using System;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class LogHelper
    {
        public static void WriteLog(string message)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "write LogFile string \"" + message + "\"");
        }

        public static void WriteLog(Exception ex)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "write LogFile exception \"" + ex.ToString() + "\"");
        }
    }
}