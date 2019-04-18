using Main.Wpf.Functions;
using System.Diagnostics;
using System.Windows;

namespace Main.Wpf.Pages
{
    public partial class Error
    {
        public Error()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Informations.Extension.IssueTracker);
        }
    }
}