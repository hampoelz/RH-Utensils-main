using Main.Wpf.ExampleExtension.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace Main.Wpf.ExampleExtension.Pages
{
    public partial class Home : Page
    {
        public static TextBlock File = new TextBlock();

        public Home()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            File = _File;

            if (string.IsNullOrEmpty(File.Text)) File.Text = "Nicht gefunden!";
        }

        private void GoToSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set selectionIndex \"3\"");
        }
    }
}