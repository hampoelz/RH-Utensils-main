using Main.Wpf.ExampleExtension.Utilities;
using System;
using System.Windows;

namespace Main.Wpf.ExampleExtension
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessageHelper.ReceiveDataMessages();

            string[] args = Environment.GetCommandLineArgs();

            for (var arg = 1; arg != args.Length; ++arg)
            {
                switch (args[arg])
                {
                    case "-page":
                        Index.Navigate(new Uri("Pages/" + args[arg + 1] + ".xaml", UriKind.Relative));
                        continue;

                        //other cases
                }
            }
        }
    }
}