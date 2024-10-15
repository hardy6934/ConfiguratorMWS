using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Buisness.Service;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.UI.MWS.MWSTabs;
using ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.Calibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.Information;
using ConfiguratorMWS.UI.MWS.MWSTabs.Settings;
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
                services.AddSingleton<MWSWindow>(); 
                services.AddSingleton<SettingsView>(); 
                services.AddSingleton<InformationView>(); 
                services.AddSingleton<CalibrationView>(); 
                services.AddSingleton<CalculatedCalibrationView>(); 

                //Repositories
                services.AddSingleton<IMWSRepository, MWSRepository>();

                //Services
                services.AddSingleton<IMWSService, MWSService>(); 

                //Models
                services.AddSingleton<MWSEntity>();

                //ViewModels
                services.AddSingleton<IMWSViewModel, MWSViewModel>(); 
                services.AddSingleton<IInformationViewModel, InformationViewModel>(); 
                services.AddSingleton<ISettingsViewModel, SettingsViewModel>(); 
                services.AddSingleton<ICalibrationViewModel, CalibrationViewModel>(); 
                services.AddSingleton<ICalculatedCalibrationViewModel, CalculatedCalibrationViewModel>(); 
            }).Build();

        }

        protected override async void OnStartup(StartupEventArgs e) { 
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MWSWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e) {  
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

     }

}
