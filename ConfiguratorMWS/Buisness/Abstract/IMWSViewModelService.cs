using ConfiguratorMWS.Entity; 

namespace ConfiguratorMWS.Buisness.Abstract
{
    public interface IMWSViewModelService
    {
        public MWSEntity GetEntity();

        public void ChangeProgressBarValue(int value);

    }
}
