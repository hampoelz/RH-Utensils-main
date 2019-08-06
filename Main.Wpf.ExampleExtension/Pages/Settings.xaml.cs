using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Main.Wpf.ExampleExtension.Utilities;
using MaterialDesignThemes.Wpf.Transitions;
using Microsoft.Win32;

namespace Main.Wpf.ExampleExtension.Pages
{
    public partial class Settings
    {
        public static ToggleButton TestProperty = new ToggleButton();
        public static ToggleButton ThemeProperty = new ToggleButton();
        public static ComboBox ColorProperty = new ComboBox();

        public Settings()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TestProperty = _TestProperty;
            ThemeProperty = _ThemeProperty;
            ColorProperty = _ColorProperty;

            var colors = new List<string>
            {
                "Yellow", "Amber", "Deep Orange", "Light Blue", "Teal", "Cyan", "Pink", "Green", "Deep Purple",
                "Indigo", "Light Green", "Blue", "Lime", "Red", "Orange", "Purple"
            };

            foreach (var color in colors) ColorProperty.Items.Add(color);
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var content = "";

            for (var item = 0; item < IcSettings.Items.Count; item++)
            {
                var uiElement = (UIElement) IcSettings.ItemContainerGenerator.ContainerFromIndex(item);
                if (!(uiElement is TransitioningContent tc)) continue;

                if (!(VisualTreeHelper.GetOffset(tc).Y <= e.VerticalOffset)) continue;
                var uiGrid = (UIElement) tc.Content;
                if (!(uiGrid is Grid grid)) continue;

                for (var children = 0; children < grid.Children.Count; children++)
                {
                    var uiTextBox = grid.Children[children];

                    if (!(uiTextBox is TextBlock tb)) continue;

                    content = " / " + tb.Text;
                }
            }

            if (TbNavigation.Text == Title + content) return;

            var da2 = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            TbNavigation.BeginAnimation(OpacityProperty, da2);

            await Task.Delay(TimeSpan.FromSeconds(0.4));

            TbNavigation.Text = Title + content;

            var da1 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            TbNavigation.BeginAnimation(OpacityProperty, da1);
        }

        private void TestProperty_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("test", false.ToString());
        }

        private void TestProperty_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("test", true.ToString());
        }

        private void Privacy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://hampoelz.net/impressum/");
        }

        #region Theme

        private void ThemeProperty_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("theme", "dark");
        }

        private void ThemeProperty_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("theme", "light");
        }

        #endregion Theme

        #region Color

        private void ColorProperty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                "set tmp Color \"" + ColorProperty.SelectedItem + "\"");
        }

        private void ColorProperty_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                "set Color \"" + ColorProperty.SelectedItem + "\"");
        }

        #endregion Color

        #region Favicon

        private string _favicon = "";

        private void FaviconProperty_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Grafik Dateien (*.png)|*.png"
            };

            if (openDialog.ShowDialog() != true) return;
            _favicon = openDialog.FileName;

            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp Favicon \"" + _favicon + "\"");
        }

        private void FaviconProperty_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set Favicon \"" + _favicon + "\"");
        }

        #endregion Favicon

        #region Site

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                "add tmp Site \"null\" \"null\" \"null\" \"null\"");
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                "add tmp Site \"Test Page\" \"TestTube\" \"notepad.exe\" \"null\"");

            Add.IsEnabled = false;
            Update.IsEnabled = true;
            Remove.IsEnabled = true;
            GoTo.IsEnabled = true;
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                "update tmp Site \"5\" \"New Test\" \"TestTubeOff\" \"powershell.exe\" \"null\"");

            Add.IsEnabled = false;
            Update.IsEnabled = false;
            Remove.IsEnabled = true;
            GoTo.IsEnabled = true;
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "remove tmp Site \"4\"");
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "remove tmp Site \"4\"");

            Add.IsEnabled = true;
            Update.IsEnabled = false;
            Remove.IsEnabled = false;
            GoTo.IsEnabled = false;
        }

        private void GoTo_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp selectionIndex \"5\"");
        }

        #endregion Site

        #region Size

        private int _height = 700;
        private int _width = 1200;

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            if (_height != 800 && _width != 1300)
            {
                _height = 800;
                _width = 1300;

                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                    "set tmp WindowHeight \"" + _height + "\"");
                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                    "set tmp WindowWidth \"" + _width + "\"");
            }
            else
            {
                _height = 700;
                _width = 1200;

                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                    "set tmp WindowHeight \"" + _height + "\"");
                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(),
                    "set tmp WindowWidth \"" + _width + "\"");
            }
        }

        private void ChangeSize_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set WindowHeight \"" + _height + "\"");
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set WindowWidth \"" + _width + "\"");
        }

        #endregion Size
    }
}