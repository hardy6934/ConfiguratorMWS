using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.Data.Repository
{
    public class MWSRepository : IMWSRepository
    { 
        private MWSEntity mWSEntity;  
        
        public MWSRepository() {
            mWSEntity = new MWSEntity();
            Array.Fill(mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, (byte)0xFF);
        }


        public MWSEntity GetEntity()
        {
            return mWSEntity;
        }
        public void ChangeProgressBarValue(int value)
        {
            mWSEntity.ProgressValue = value;
        }
        public void IncremeentingProgressBarValue(int value)
        {
            mWSEntity.ProgressValue += value;
        }

        public void ChangeUpdatingProgressBarValue(int value)
        {
            mWSEntity.UpdatingProgressValue = value;
        }

        public void UpdateWindowProgresBarStatus(string val)
        {
            mWSEntity.UpdateWindowProgresBarStatus = val;
        }
        public void GeneralWindowProgressBarStatus(string val) 
        { 
            mWSEntity.GeneralWindowProgressBarStatus = val;
        }

    }
}
