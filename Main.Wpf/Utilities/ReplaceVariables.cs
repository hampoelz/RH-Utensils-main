using System;

namespace Main.Wpf.Utilities
{
    public static class ReplaceVariables
    {
        public static string Replace(string value)
        {
            if (string.IsNullOrEmpty(Config.ExtensionDirectoryName)) return value;

            value = value.Replace("{extensionsDirectory}", Config.ExtensionsDirectory);
            value = value.Replace("{extensionName}", Config.ExtensionDirectoryName);
            value = value.Replace("{extensionVersion}", Config.Updater.Extension.RunningVersion.ToString());
            value = value.Replace("{appName}", Config.Informations.Extension.Name);
            value = value.Replace("{username}", Environment.UserName);

            return value;
        }
    }
}