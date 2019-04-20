using System;
using System.Windows;

namespace Main.Wpf.ExampleExtension.Functions
{
    internal class Index
    {
        public static void Set(string page)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            mw.Index.Navigate(new Uri("Pages/" + page + ".xaml", UriKind.Relative));
        }
    }
}