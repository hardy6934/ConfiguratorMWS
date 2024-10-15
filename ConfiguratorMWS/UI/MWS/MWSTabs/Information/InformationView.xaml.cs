using System.Windows.Controls; 

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Information
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class InformationView : UserControl
    {
        private readonly IInformationViewModel informationViewModel;
        public InformationView(IInformationViewModel informationViewModel)
        {
            InitializeComponent();
            DataContext = informationViewModel;
        }
    }
}
