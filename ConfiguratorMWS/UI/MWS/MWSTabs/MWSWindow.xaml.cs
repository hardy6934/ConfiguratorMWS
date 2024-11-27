using System.Windows;
using System.Windows.Controls; 
using ConfiguratorMWS.Resources; 
using ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware;
using Microsoft.Extensions.DependencyInjection;

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
            DataContext = mWSViewModel; 


            if (mWSViewModel is MWSViewModel viewModel)
            {
                viewModel.ParentWindow = this;
            }
              
        }
         
        private void SecondWindow_Closed(object? sender, EventArgs e)
        { 
            this.Show();
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
