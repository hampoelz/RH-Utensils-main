using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Main.Wpf.Functions
{
    public static class Index
    {
        public static void SetIndex(string page)
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            mw.Index.Navigate(new Uri("Pages/" + page + ".xaml", UriKind.Relative));
            mw.Index.Visibility = Visibility.Visible;
            mw.IndexGrid.Visibility = Visibility.Collapsed;
        }

        public static async Task SetExeAsync(string path, string argument, int loadTime)
        {
            if (Application.Current.MainWindow is MainWindow mw)
                await mw.SetExe(path, argument, loadTime);
        }

        public static void SetError(string exception, string title, string file = "-")
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var errorFrame = new Frame();
            mw.Grid.Children.Add(errorFrame);

            errorFrame.Navigate(new Uri("Pages/Error.xaml", UriKind.Relative));

            mw.Menu.Visibility = Visibility.Collapsed;
            mw.Index.Visibility = Visibility.Collapsed;
            mw.IndexGrid.Visibility = Visibility.Collapsed;

            Pages.Error.ErrorMessage = exception;
            Pages.Error.File = file;
            Pages.Error.Title = title;
        }
    }
}