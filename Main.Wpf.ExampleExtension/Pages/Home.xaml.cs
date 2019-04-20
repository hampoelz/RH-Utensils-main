using Main.Wpf.ExampleExtension.Functions;
using System.Windows.Controls;

namespace Main.Wpf.ExampleExtension.Pages
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (App.File != null) File.Text = App.File;

            if (Functions.Settings.GetProperty("theme", "dark") == "dark")
                Functions.Settings.SetTheme("dark");
            else
                Functions.Settings.SetTheme("light");
        }

        private async void GoToSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await Config.SetString("config/selectionIndex", "3").ConfigureAwait(false);
        }
    }
}