using System;

namespace Main.Wpf.Functions
{
    public static class ReplaceVariables
    {
        public static string Replace(string value)
        {
            if (Config.ExtensionDirectoryName?.Length == 0) return value;

            return value.Replace("{extensionsDirectory}", Config.ExtensionsDirectory)
                .Replace("{extensionName}", Config.ExtensionDirectoryName)
                .Replace("{extensionVersion}", Updater.Informations.Extension.RunningVersion.ToString())
                .Replace("{appName}", Informations.Extension.Name)
                .Replace("{username}", Environment.UserName);
        }
    }
}