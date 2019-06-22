using System;
using System.Windows;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class IndexHelper
    {
        public static void SetIndex(string page)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            mw.Index.Navigate(new Uri("Pages/" + page + ".xaml", UriKind.Relative));
        }
    }
}