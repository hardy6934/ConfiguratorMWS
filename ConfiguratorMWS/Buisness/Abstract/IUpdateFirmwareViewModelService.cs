

using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.Buisness.Abstract
{
    public interface IUpdateFirmwareViewModelService
    {
        public void ChangeTimerWorkInterval(int interval);
        public MWSEntity GetEntity();
        public void ChangeProgressBarValue(int value);
        public void ChangeUpdateProgressBarValue(int value);
        public void UpdateWindowProgresBarStatus(string val);

    }
}
