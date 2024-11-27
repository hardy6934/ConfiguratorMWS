
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Settings
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly ISettingsViewModel viewModel;
        public SettingsView(ISettingsViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void ToggleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            TogglePodtyazhka.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#0386ce");
            InideRadioInTogglePodtyazhka.Background = new SolidColorBrush(Colors.White);
            InideRadioInTogglePodtyazhka.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
        }

        private void TogglePodtyazhka_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            TogglePodtyazhka.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#676767");
            InideRadioInTogglePodtyazhka.Background = new SolidColorBrush(Colors.White);
            InideRadioInTogglePodtyazhka.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
        private void InideRadioInTogglePodtyazhka_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (TogglePodtyazhka.IsChecked == true)
            {
                TogglePodtyazhka.IsChecked = false;
            }
            else
            {
                TogglePodtyazhka.IsChecked = true;
            }
        }


        private void ToggleTerminator_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleTerminator.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#0386ce");
            InideRadioInToggleTerminator.Background = new SolidColorBrush(Colors.White);
            InideRadioInToggleTerminator.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
        }

        private void ToggleTerminator_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleTerminator.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#676767");
            InideRadioInToggleTerminator.Background = new SolidColorBrush(Colors.White);
            InideRadioInToggleTerminator.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }

        private void InideRadioInToggleTerminator_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ToggleTerminator.IsChecked == true)
            {
                ToggleTerminator.IsChecked = false;
            }
            else
            {
                ToggleTerminator.IsChecked = true;
            }
        }

    }
}
