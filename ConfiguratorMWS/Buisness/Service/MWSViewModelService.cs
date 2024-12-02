

using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.UI.MWS.MWSModals;
using ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ConfiguratorMWS.Buisness.Service
{
    public class MWSViewModelService : IMWSViewModelService
    {
        private readonly IMWSRepository mwsRepository;
        private readonly IServiceProvider serviceProvider;

        public MWSViewModelService(IMWSRepository mwsRepository, IServiceProvider serviceProvider)
        {
            this.mwsRepository = mwsRepository;
            this.serviceProvider = serviceProvider;
        }

        public void ChangeProgressBarValue(int value)
        {
            mwsRepository.ChangeProgressBarValue(value);
        }

        public MWSEntity GetEntity()
        {
            return mwsRepository.GetEntity();
        }

        public void ClearToken()
        {
            Properties.Settings.Default.Token = "";
            Properties.Settings.Default.TokenAccountId = "";
            Properties.Settings.Default.TokenExpDate = "";
            Properties.Settings.Default.TokenRole = "";
            Properties.Settings.Default.Save();
        }

        

        public void ShowUpdateFirmwareWindow(Window parentWindow)
        {
            var updateFirmwareWindow = serviceProvider.GetRequiredService<UpdateFirmwareWindow>();
            updateFirmwareWindow.Owner = parentWindow;
            updateFirmwareWindow.Closed += (sender, e) =>
            {
                // Показываем родительское окно при закрытии
                parentWindow.Show();
            };

            parentWindow.Hide(); // Скрываем родительское окно
            updateFirmwareWindow.Show();
        }

        public void ShowAuthorizationModal(Window parentWindow)
        {
            var authorizationModalWindow = serviceProvider.GetRequiredService<AuthorizationModalWindow>();
            authorizationModalWindow.Owner = parentWindow;
            authorizationModalWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            authorizationModalWindow.ShowDialog();
        }

    }
}
