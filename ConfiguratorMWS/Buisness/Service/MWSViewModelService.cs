

using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.Buisness.Service
{
    public class MWSViewModelService : IMWSViewModelService
    {
        private readonly IMWSRepository mwsRepository;

        public MWSViewModelService(IMWSRepository mwsRepository)
        {
            this.mwsRepository = mwsRepository;
        }

        public void ChangeProgressBarValue(int value)
        {
            mwsRepository.ChangeProgressBarValue(value);
        }

        public MWSEntity GetEntity()
        {
            return mwsRepository.GetEntity();
        }
    }
}
