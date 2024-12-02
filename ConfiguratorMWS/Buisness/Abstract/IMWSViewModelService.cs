using ConfiguratorMWS.Entity;
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

    }
}
