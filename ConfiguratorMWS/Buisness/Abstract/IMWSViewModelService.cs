using ConfiguratorMWS.Entity;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Windows;
namespace ConfiguratorMWS.Buisness.Abstract
{
    public interface IMWSViewModelService
    {
        public MWSEntity GetEntity();
        public void ChangeProgressBarValue(int value);
        public void ClearToken();

        public void ShowUpdateFirmwareWindow(Window parentWindow);
        public void ShowAuthorizationModal(Window parentWindow);


        public void EnsurePendingRequestsFolderExists();
        public Task RetryPendingRequestsAsync();

    }
}
