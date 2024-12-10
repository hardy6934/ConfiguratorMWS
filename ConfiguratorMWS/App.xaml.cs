using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Buisness.Service;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Properties;
using ConfiguratorMWS.Resources;
using ConfiguratorMWS.UI.MWS.MWSModals;
using ConfiguratorMWS.UI.MWS.MWSTabs;
using ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.Calibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.Information;
using ConfiguratorMWS.UI.MWS.MWSTabs.Settings;
using ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; 
using System.Windows;

namespace ConfiguratorMWS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IHost? AppHost { get; private set; }

        public App() {

            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<MWSWindow>();
                services.AddTransient<UpdateFirmwareWindow>(); 
                services.AddTransient<AuthorizationModalWindow>();

                //Repositories
                services.AddSingleton<IMWSRepository, MWSRepository>();
                services.AddSingleton<ISerialPortRepository, SerialPortRepository>();
                services.AddSingleton<ITimerRepository, TimerRepository>();

                //Services
                services.AddSingleton<IInformationViewModelService, InformationViewModelService>();  
                services.AddSingleton<ISettingsViewModelService, SettingsViewModelService>();  
                services.AddSingleton<IMWSViewModelService, MWSViewModelService>();  
                services.AddSingleton<IUpdateFirmwareViewModelService, UpdateFirmwareViewModelService>();  
                  
                //ViewModels
                services.AddSingleton<IMWSViewModel, MWSViewModel>(); 
                services.AddSingleton<IInformationViewModel, InformationViewModel>(); 
                services.AddSingleton<ISettingsViewModel, SettingsViewModel>(); 
                services.AddSingleton<ICalibrationViewModel, CalibrationViewModel>(); 
                services.AddSingleton<ICalculatedCalibrationViewModel, CalculatedCalibrationViewModel>(); 
                services.AddSingleton<IUpdateFirmwareViewModel, UpdateFirmwareViewModel>(); 


            }).Build();

        }

        protected override async void OnStartup(StartupEventArgs e) { 
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MWSWindow>();
            startupForm.Show();

            base.OnStartup(e);


            // Подписка на событие LanguageChanged
            LocalizedStrings.LanguageChanged += () =>
            {
                // Уведомляем интерфейс о смене всех текстовых ресурсов
                (Resources["LocalizedStrings"] as LocalizedStrings)?.OnPropertyChanged(null);
            };
            
            var mwsViewModelService = AppHost.Services.GetRequiredService<IMWSViewModelService>();
            mwsViewModelService.EnsurePendingRequestsFolderExists();
            await mwsViewModelService.RetryPendingRequestsAsync();
        }
        protected override async void OnExit(ExitEventArgs e) {  
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

     }

}
