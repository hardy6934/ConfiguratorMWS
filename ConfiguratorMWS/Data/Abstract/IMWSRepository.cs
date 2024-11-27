using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity; 

namespace ConfiguratorMWS.Data.Abstract
{
    public interface IMWSRepository
    {

        public MWSEntity GetEntity();

        //public void DecodeIntResponse(byte[] bytes); 
        //public void DecodeBootloader(byte[] bytes);
        public void ChangeProgressBarValue(int value);
        public void IncremeentingProgressBarValue(int value);
        public void ChangeUpdatingProgressBarValue(int value);
        public void UpdateWindowProgresBarStatus(string val);
        public void GeneralWindowProgressBarStatus(string val);

    }
}
