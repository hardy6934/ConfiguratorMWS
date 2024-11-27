using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.Buisness.Service
{
    public class UpdateFirmwareViewModelService : IUpdateFirmwareViewModelService
    {
        private readonly IMWSRepository mWSRepository;
        private readonly ITimerRepository timerRepository; 

        public UpdateFirmwareViewModelService(IMWSRepository mWSRepository, ITimerRepository timerRepository)
        {
            this.mWSRepository = mWSRepository;
            this.timerRepository = timerRepository; 
        }
        public MWSEntity GetEntity()
        {
            return mWSRepository.GetEntity();
        }
        public void ChangeProgressBarValue(int value)
        {
            mWSRepository.ChangeProgressBarValue(value);
        }
        public void ChangeUpdateProgressBarValue(int value)
        {
            mWSRepository.ChangeUpdatingProgressBarValue(value);
        }
        public void ChangeTimerWorkInterval(int interval)
        {
            timerRepository.ChangeTimerWorkInterval(interval);
        }
        public void UpdateWindowProgresBarStatus(string val)
        {
            mWSRepository.UpdateWindowProgresBarStatus(val);
        }


    }
}
