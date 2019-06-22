using Main.Wpf.ExampleExtension.Utilities;
using MaterialDesignThemes.Wpf.Transitions;
using System;
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

        public Settings()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TestControl.Children.Add(TestProperty);
            TestProperty.Checked += TestProperty_Checked;
            TestProperty.Unchecked += TestProperty_Unchecked;

            ThemeControl.Children.Add(ThemeProperty);
            ThemeProperty.Checked += ThemeProperty_Checked;
            ThemeProperty.Unchecked += ThemeProperty_Unchecked;
        }

        private void ThemeProperty_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("theme", "dark");
        }

        private void ThemeProperty_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("theme", "light");
        }

        private void TestProperty_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("test", false.ToString());
        }

        private void TestProperty_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeValue("test", true.ToString());
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

        private void Privacy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://hampoelz.net/impressum/");
        }
    }
}