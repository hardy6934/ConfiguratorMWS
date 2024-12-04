using ConfiguratorMWS.Base;
using ConfiguratorMWS.UI.MWS.MWSTabs.Information;
using ConfiguratorMWS.UI.MWS.MWSTabs.Settings;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.UI.MWS.MWSTabs.Calibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration;
using Microsoft.Extensions.DependencyInjection;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Resources;
using System.Collections.ObjectModel;
using System.Windows;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    public class MWSViewModel : ViewModelBase, IMWSViewModel
    {

        private object currentView;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMWSViewModelService mWSViewModelService;
         
        // Команда для переключения вкладок
        public RelayCommand SwitchTabCommand { get; }
        public RelayCommand ChangeLanguageCommand { get; } 
        public RelayCommand LogOutCommand { get; }
        public RelayCommand OpenUpdateFirmwareWindowCommand { get; }
        public RelayCommand OpenAuthorizationModalCommand { get; }
        public RelayCommand ShowAuthorizationModalCommand { get; } 

        //entity
        public MWSEntity mWSEntity { get; set; }

        public ObservableCollection<string> Languages { get; }
        public string SelectedLanguage { get; set; }   

        public MWSViewModel(IServiceProvider serviceProvider, IMWSViewModelService mWSViewModelService)
        {
            _serviceProvider = serviceProvider;
            this.mWSViewModelService = mWSViewModelService;
            this.mWSEntity = mWSViewModelService.GetEntity();

            //изменение и сохранение языков
            Languages = new ObservableCollection<string> { "РУС", "ENG"};
            SelectedLanguage = Properties.Settings.Default.Lang ?? "РУС";
            LocalizedStrings.SetLanguage(SelectedLanguage);
            //изменение и сохранение языков

            // Инициализация команд
            SwitchTabCommand = new RelayCommand(SwitchTab);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);
            LogOutCommand = new RelayCommand(LogOut); 
            OpenUpdateFirmwareWindowCommand = new RelayCommand(OpenUpdateFirmwareWindow, (obj) => mWSEntity.isConnected);
            OpenAuthorizationModalCommand = new RelayCommand(OpenAuthorizationModal);

            // Устанавливаем начальное содержимое
            CurrentView = new InformationView(_serviceProvider.GetRequiredService<IInformationViewModel>());

            mWSViewModelService.ChangeProgressBarValue(0);
        }

        //Залогинен или нет
        public bool IsLoggedIn
        {
            get
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.Token) && !string.IsNullOrEmpty(Properties.Settings.Default.TokenAccountId))
                { 
                    DateTime tokenExpDate;

                    if (DateTime.TryParse(Properties.Settings.Default.TokenExpDate, out tokenExpDate))
                    {
                        DateTime currentDate = DateTime.Now;

                        return tokenExpDate > currentDate; 
                    }
                }
                return false;
            }
        }


        // Текущее содержимое, которое отображается в нижней части окна
        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                RaisePropertyChanged("CurrentView");
            }
        }
          
        // Логика переключения вкладок
        public void SwitchTab(object tab)
        {
            switch (tab)
            {
                case "Information":
                    CurrentView = new InformationView(_serviceProvider.GetRequiredService<IInformationViewModel>());
                    break;
                case "Settings":
                    CurrentView = new SettingsView(_serviceProvider.GetRequiredService<ISettingsViewModel>());
                    break;
                case "Calibration":
                    CurrentView = new CalibrationView(_serviceProvider.GetRequiredService<ICalibrationViewModel>());
                    break;
                case "CalculatedCalibration":
                    CurrentView = new CalculatedCalibrationView(_serviceProvider.GetRequiredService<ICalculatedCalibrationViewModel>());
                    break;
            }
        }
         

        private void ChangeLanguage(object language)
        {
            if (language != null)
            {
                string cultureCode = language.ToString();

                LocalizedStrings.SetLanguage(cultureCode); 

                Properties.Settings.Default.Lang = cultureCode;
                Properties.Settings.Default.Save();
            }
        }

        // Метод для открытия нового окна прошивки
        private void OpenUpdateFirmwareWindow(object parametr)
        {
            if (parametr is Window parentWindow)
            {
                mWSViewModelService.ShowUpdateFirmwareWindow(parentWindow);
            }

        } 

        private void OpenAuthorizationModal(object parametr)
        {
            if (parametr is Window parentWindow)
            {
                mWSViewModelService.ShowAuthorizationModal(parentWindow);
                RaisePropertyChanged(nameof(IsLoggedIn));
            } 
        }

        private void LogOut(object obj)
        {
            mWSViewModelService.ClearToken(); 
            RaisePropertyChanged(nameof(IsLoggedIn));
        }

       
    }
}
