using ConfiguratorMWS.UI.MWS.MWSModals;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration
{
    /// <summary>
    /// Interaction logic for CalculatedCalibrationView.xaml
    /// </summary>
    public partial class CalculatedCalibrationView : UserControl
    {
        private readonly ICalculatedCalibrationViewModel viewModel;
        public CalculatedCalibrationView(ICalculatedCalibrationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
          
        private void ButtonModalHowToFill_Click(object sender, RoutedEventArgs e)
        {
            var modal = new MWSModalWindow();
            var parentWindow = Window.GetWindow(this);
            modal.Owner = parentWindow;
            modal.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            modal.ShowDialog();
        }
         
    }

    
}
