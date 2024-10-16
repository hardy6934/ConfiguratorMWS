using System.ComponentModel;
using System.Windows;
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

         
    //private void DecreaseFuel_Click(object sender, RoutedEventArgs e)
    //{
    //    if (informationViewModel.FuelLevel > 0)
    //    {
    //            informationViewModel.FuelLevel -= 30;  // Уменьшаем уровень топлива на 30 (пример)
    //    }
    //}
      

    }
}
