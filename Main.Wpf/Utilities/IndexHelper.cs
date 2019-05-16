using System;
using System.Windows;

namespace Main.Wpf.Utilities
{
    public static class IndexHelper
    {
        public static void SetIndex(string page)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            mw.Index.Navigate(new Uri("Pages/" + page + ".xaml", UriKind.Relative));
            mw.Index.Visibility = Visibility.Visible;
            mw.IndexGrid.Visibility = Visibility.Collapsed;
        }
    }
}