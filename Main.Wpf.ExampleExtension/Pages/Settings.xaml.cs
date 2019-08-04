using Main.Wpf.ExampleExtension.Utilities;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Main.Wpf.ExampleExtension.Pages
{
    public partial class Settings : Page
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

            var colors = new List<string> { "Yellow", "Amber", "Deep Orange", "Light Blue", "Teal", "Cyan", "Pink", "Green", "Deep Purple", "Indigo", "Light Green", "Blue", "Lime", "Red", "Orange", "Purple" };

            foreach (string color in colors)
            {
                ColorProperty.Items.Add(color);
            }
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var content = "";

            for (int item = 0; item < IC_Settings.Items.Count; item++)
            {
                UIElement uiElement = (UIElement)IC_Settings.ItemContainerGenerator.ContainerFromIndex(item);
                if (!(uiElement is TransitioningContent TC)) continue;

                if (VisualTreeHelper.GetOffset(TC).Y <= e.VerticalOffset)
                {
                    UIElement uiGrid = (UIElement)TC.Content;
                    if (!(uiGrid is Grid grid)) continue;

                    for (int children = 0; children < grid.Children.Count; children++)
                    {
                        UIElement uiTextBox = (UIElement)grid.Children[children];

                        if (!(uiTextBox is TextBlock TB)) continue;

                        content = " / " + TB.Text;
                    }
                }
            }

            if (TB_Navigation.Text == Title + content) return;

            DoubleAnimation DA2 = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            TB_Navigation.BeginAnimation(OpacityProperty, DA2);

            await Task.Delay(TimeSpan.FromSeconds(0.4));

            TB_Navigation.Text = Title + content;

            DoubleAnimation DA1 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            TB_Navigation.BeginAnimation(OpacityProperty, DA1);
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
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp Color \"" + ColorProperty.SelectedItem + "\"");
        }

        private void ColorProperty_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set Color \"" + ColorProperty.SelectedItem + "\"");
        }

        #endregion Color

        #region Favicon

        private string Favicon = "";

        private void FaviconProperty_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Grafik Dateien (*.png)|*.png"
            };
            if (openDialog.ShowDialog() == true)
            {
                Favicon = openDialog.FileName;

                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp Favicon \"" + Favicon + "\"");
            }
        }

        private void FaviconProperty_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set Favicon \"" + Favicon + "\"");
        }

        #endregion Favicon

        #region Site

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "add tmp Site \"null\" \"null\" \"null\" \"null\"");
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "add tmp Site \"Test Page\" \"TestTube\" \"notepad.exe\" \"null\"");

            Add.IsEnabled = false;
            Update.IsEnabled = true;
            Remove.IsEnabled = true;
            GoTo.IsEnabled = true;
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "update tmp Site \"5\" \"New Test\" \"TestTubeOff\" \"powershell.exe\" \"null\"");

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

        private int _Height = 700;
        private int _Width = 1200;

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            if (_Height != 800 && _Width != 1300)
            {
                _Height = 800;
                _Width = 1300;

                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp WindowHeight \"" + _Height + "\"");
                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp WindowWidth \"" + _Width + "\"");
            }
            else
            {
                _Height = 700;
                _Width = 1200;

                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp WindowHeight \"" + _Height + "\"");
                MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set tmp WindowWidth \"" + _Width + "\"");
            }
        }

        private void ChangeSize_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set WindowHeight \"" + _Height + "\"");
            MessageHelper.SendDataMessage(InstanceHelper.GetMainProcess(), "set WindowWidth \"" + _Width + "\"");
        }

        #endregion Size
    }
}