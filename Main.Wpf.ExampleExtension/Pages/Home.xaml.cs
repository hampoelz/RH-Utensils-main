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
            if (string.IsNullOrEmpty(File.Text)) File.Text = "Nicht gefunden!";
            File.FontSize = 20;
            File.TextAlignment = TextAlignment.Center;
            File.VerticalAlignment = VerticalAlignment.Center;
            File.TextWrapping = TextWrapping.WrapWithOverflow;
            Content.Children.Add(File);
        }

        private void GoToSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set selectionIndex \"3\"");
        }
    }
}