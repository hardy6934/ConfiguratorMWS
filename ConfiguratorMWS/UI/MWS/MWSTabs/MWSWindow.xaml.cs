using System.Windows;
using System.Windows.Controls; 
using ConfiguratorMWS.Resources;
using ConfiguratorMWS.UI.MWS.MWSModals;
using ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    /// <summary>
    /// Interaction logic for MWSWindow.xaml
    /// </summary>
    public partial class MWSWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IMWSViewModel mWSViewModel; 
        public MWSWindow(IMWSViewModel mWSViewModel)
        {
            InitializeComponent();
            this.mWSViewModel = mWSViewModel;
            DataContext = mWSViewModel;
            mWSViewModel.UpdateProgramStuperFromRemoteServerCommand.Execute(this);
        }

        private void LanguageComboBox_Selected(object sender, RoutedEventArgs e)
        { 
            if (LanguageComboBox.SelectedItem is string selectedLanguage)
            {
                var viewModel = (MWSViewModel)DataContext;
                viewModel.ChangeLanguageCommand.Execute(selectedLanguage);
            }
        }  

      
    } 
     
}
