using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;
using System.Collections.ObjectModel; 

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    public interface IMWSViewModel
    {
        MWSEntity mWSEntity { get; set; }
        RelayCommand SwitchTabCommand { get; }
        RelayCommand ChangeLanguageCommand { get; }
        RelayCommand LogOutCommand { get; }
        RelayCommand OpenUpdateFirmwareWindowCommand { get; }
        RelayCommand OpenAuthorizationModalCommand { get; }
        RelayCommand ShowAuthorizationModalCommand { get; }
        RelayCommand UpdateProgramStuperFromRemoteServerCommand { get; }

        object CurrentView { get; set; }
        void SwitchTab(object tab);

        ObservableCollection<string> Languages { get; }
        string SelectedLanguage { get; set; }
        bool IsLoggedIn { get; }
        public string CurrentVersion { get; set; }
    }
}
