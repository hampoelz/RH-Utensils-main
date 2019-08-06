using System.Windows;
using System.Windows.Controls;
using Main.Wpf.ExampleExtension.Utilities;

namespace Main.Wpf.ExampleExtension.Pages
{
    public partial class Home
    {
        public static TextBlock File = new TextBlock();

        public Home()
        {
            InitializeComponent();

            File = _File;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(File.Text)) File.Text = "Nicht gefunden!";
        }

        private void GoToSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set selectionIndex \"3\"");
        }
    }
}