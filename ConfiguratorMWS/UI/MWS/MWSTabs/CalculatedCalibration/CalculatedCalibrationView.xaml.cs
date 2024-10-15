
using System.Windows.Controls;

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
    }
}
