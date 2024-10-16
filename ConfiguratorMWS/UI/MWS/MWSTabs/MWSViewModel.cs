using ConfiguratorMWS.Base;
using ConfiguratorMWS.UI.MWS.MWSTabs.Information;
using ConfiguratorMWS.UI.MWS.MWSTabs.Settings;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.UI.MWS.MWSTabs.Calibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration;
using Microsoft.Extensions.DependencyInjection;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using System.IO.Ports;
using ConfiguratorMWS.Entity;
using System.Windows;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    public class MWSViewModel : ViewModelBase, IMWSViewModel
    {

        private object currentView;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMWSRepository mWSRepository;
         
        // Команда для переключения вкладок
        public RelayCommand SwitchTabCommand { get; }
         
        //entity
        private MWSEntity mWSEntity;
          
        public MWSViewModel(IServiceProvider serviceProvider, IMWSService mWSService, IMWSRepository mWSRepository, MWSEntity mWSEntity)
        {
            _serviceProvider = serviceProvider;
            this.mWSRepository = mWSRepository;
            this.mWSEntity = mWSEntity;

             
            // Инициализация команды
            SwitchTabCommand = new RelayCommand(SwitchTab); 

            // Устанавливаем начальное содержимое
            CurrentView = new InformationView(_serviceProvider.GetRequiredService<IInformationViewModel>());
             
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


          
       

       




    }
}
