
using System.Windows.Controls;

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
            DataContext = viewModel;
        }
    }
}
