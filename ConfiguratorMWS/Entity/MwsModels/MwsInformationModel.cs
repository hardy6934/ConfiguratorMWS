

using ConfiguratorMWS.Base;
using ConfiguratorMWS.Entity.MWSSubModels;

namespace ConfiguratorMWS.Entity.MwsModels
{
    public class MwsInformationModel: ObservableBase
    {
        private MwsCommonDataClass mwsCommonData;
        public MwsCommonDataClass MwsCommonData
        {
            get
            {
                return mwsCommonData;
            }
            set
            {
                mwsCommonData = value;
                RaisePropertyChanged(nameof(MwsCommonData));
            }

        }

        //постоянные данные 71 команда
        private MwsRealTimeDataClass mwsRealTimeData;
        public MwsRealTimeDataClass MwsRealTimeData
        {
            get
            {
                return mwsRealTimeData;
            }
            set
            {
                mwsRealTimeData = value;
                RaisePropertyChanged(nameof(MwsRealTimeData));
            }

        }
    }
}
