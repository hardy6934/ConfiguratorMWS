using ConfiguratorMWS.Base;
using ConfiguratorMWS.UI.MWS.MWSTabs.Information;
using ConfiguratorMWS.UI.MWS.MWSTabs.Settings;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.UI.MWS.MWSTabs.Calibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration;
using Microsoft.Extensions.DependencyInjection;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware;
using ConfiguratorMWS.Resources;
using System.Collections.ObjectModel;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    public class MWSViewModel : ViewModelBase, IMWSViewModel
    {

        private object currentView;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMWSViewModelService mWSViewModelService;
         
        // Команда для переключения вкладок
        public RelayCommand SwitchTabCommand { get; }
        public RelayCommand OpenUpdateFirmwareWindowCommand { get; }
        public RelayCommand ChangeLanguageCommand { get; }
        public MWSWindow ParentWindow { get; set; }

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
            OpenUpdateFirmwareWindowCommand = new RelayCommand(OpenUpdateFirmwareWindow, (obj) => mWSEntity.isConnected); 
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);

            // Устанавливаем начальное содержимое
            CurrentView = new InformationView(_serviceProvider.GetRequiredService<IInformationViewModel>());

            mWSViewModelService.ChangeProgressBarValue(0);
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


        // Метод для открытия нового окна прошивки
        private void OpenUpdateFirmwareWindow(object obj)
        {
            var updateFirmwareWindow = _serviceProvider.GetRequiredService<UpdateFirmwareWindow>();

            updateFirmwareWindow.Owner = ParentWindow;   
            updateFirmwareWindow.Closed += UpdateFirmwareWindow_Closed;

            ParentWindow.Hide();  // Скрываем текущее окно
            updateFirmwareWindow.Show();
        }

        // Метод для обработки закрытия окна UpdateFirmwareWindow
        private void UpdateFirmwareWindow_Closed(object? sender, EventArgs e)
        {
            ParentWindow.Show();  // Возвращаем видимость окна MWSWindow после закрытия UpdateFirmwareWindow
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


    }
}
