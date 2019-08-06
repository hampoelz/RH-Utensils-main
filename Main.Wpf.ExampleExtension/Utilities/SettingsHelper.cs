namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class SettingsHelper
    {
        public static void ChangeValue(string parameter, string newValue)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                "change SettingProperty \"" + parameter + "\" \"" + newValue + "\"");
        }
    }
}