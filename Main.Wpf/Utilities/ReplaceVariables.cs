using System;

namespace Main.Wpf.Utilities
{
    public static class ReplaceVariables
    {
        public static string Replace(string value)
        {
            if (Config.ExtensionDirectoryName?.Length == 0) return value;

            return value.Replace("{extensionsDirectory}", Config.ExtensionsDirectory)
                .Replace("{extensionName}", Config.ExtensionDirectoryName)
                .Replace("{extensionVersion}", Config.Updater.Extension.RunningVersion.ToString())
                .Replace("{appName}", Config.Informations.Extension.Name)
                .Replace("{username}", Environment.UserName);
        }
    }
}