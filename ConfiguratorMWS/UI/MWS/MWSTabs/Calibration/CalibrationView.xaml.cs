
using ConfiguratorMWS.Entity.MWSStructs;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Calibration
{
    /// <summary>
    /// Interaction logic for CalibrationView.xaml
    /// </summary>
    public partial class CalibrationView : UserControl
    {
        private readonly ICalibrationViewModel calibrationViewModel;
        public CalibrationView(ICalibrationViewModel calibrationViewModel)
        { 
            InitializeComponent();
            DataContext = calibrationViewModel; 
            this.calibrationViewModel = calibrationViewModel;
        }
    }
}
