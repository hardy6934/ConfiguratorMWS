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
            this.informationViewModel = informationViewModel;
            DataContext = informationViewModel;
        }
        private void CommPortCombobox_DropDownOpened(object sender, EventArgs e)
        {
            informationViewModel.RefreshComPorts(); 
        }

    }
}
