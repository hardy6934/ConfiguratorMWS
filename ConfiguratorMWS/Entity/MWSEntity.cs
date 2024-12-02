using ConfiguratorMWS.Base;
using ConfiguratorMWS.Entity.MWSSubModels;
using ConfiguratorMWS.Resources;

namespace ConfiguratorMWS.Entity
{
    public class MWSEntity : ObservableBase, SensorModel
    { 
        public MWSEntity() {
            MwsConfigurationVariables = new MwsConfigurationVariablesClass();
            MwsCommonData = new MwsCommonDataClass(); 
            MwsProdSettingsClass = new MwsProdSettingsClass();
            MwsUserSettings = new MwsUserSettingsClass();
            MwsTable = new MwsTableClass();
            MwsRealTimeData = new MwsRealTimeDataClass(); 
        }
         

        public bool isConnected;
        private int commandStatus = 0;
        public int CommandStatus { //Switch case statuses
            get {
                return commandStatus;
            }
            set
            {
                commandStatus = value;
                
                RaisePropertyChanged(nameof(CommandStatus)); 
                RaisePropertyChanged(nameof(GeneralWindowProgressBarStatus));

            }
        }   

        //Progress bar
        private double progressValue;
        public double ProgressValue
        {
            get => progressValue;
            set
            {
                progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }
        private double updatingProgressValue;
        public double UpdatingProgressValue
        {
            get => updatingProgressValue;
            set
            {
                updatingProgressValue = value;
                RaisePropertyChanged(nameof(UpdatingProgressValue));
            }
        }

        //variables for (подключено, обновлено, сброшено на заваодские и т.д.) 
        private string updateWindowProgresBarStatus; 
        public string UpdateWindowProgresBarStatus
        {
            get {
                return updateWindowProgresBarStatus;
            }
            set { 
                updateWindowProgresBarStatus = value;
                RaisePropertyChanged(nameof(UpdateWindowProgresBarStatus));
            }
        }
        private string generalWindowProgressBarStatus;
        public string GeneralWindowProgressBarStatus
        {
            get
            {
                return generalWindowProgressBarStatus;
            }
            set {
                if (generalWindowProgressBarStatus != value) {
                    generalWindowProgressBarStatus = value;
                    RaisePropertyChanged(nameof(GeneralWindowProgressBarStatus));
                }
            }
        }
        //Progress bar



        //Green or red Distance

        private float[] distanceArrayForEstabilishing = new float[20];  
        private int isStable = 0;  
        public float[] DistanceArrayForEstabilishing
        {
            get { 
                return distanceArrayForEstabilishing; 
            }

            set {
                distanceArrayForEstabilishing = value; 
            }
        }
        /////0 - not table /////1 - stable /////
        public int IsStable
        {
            get { return isStable; }
            set
            {
                if (isStable != value) 
                {
                    isStable = value;
                    RaisePropertyChanged(nameof(IsStable)); 
                }
            }
        }
        //Green or red Distance

         

        //конфигурационне данные 90 команда производственные
        private MwsProdSettingsClass mwsProdSettingsClass;
        public MwsProdSettingsClass MwsProdSettingsClass
        {
            get
            {
                return mwsProdSettingsClass;
            }
            set
            {
                mwsProdSettingsClass = value;
                RaisePropertyChanged(nameof(MwsProdSettingsClass));
            }

        }

        //конфигурационне данные 90 команда пользовательские
        private MwsUserSettingsClass mwsUserSettings; 
        public MwsUserSettingsClass MwsUserSettings
        {
            get
            {
                return mwsUserSettings;
            }
            set
            {
                mwsUserSettings = value;
                RaisePropertyChanged(nameof(MwsUserSettings));
            }

        }

        //таррировка 90 команда
        private MwsTableClass mwsTable;
        public MwsTableClass MwsTable
        {
            get
            {
                return mwsTable;
            }
            set
            {
                mwsTable = value;
                RaisePropertyChanged(nameof(MwsTable));
            }

        }

        //информация о датчике 80 команда
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
         
        public MwsConfigurationVariablesClass MwsConfigurationVariables { get; set; }

          
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                isConnected = value;
                RaisePropertyChanged("IsConnected");
            }
        }




        public MWSEntity Clone()
        {
            return new MWSEntity
            {
                isConnected = this.isConnected,
                CommandStatus = this.CommandStatus,
                MwsConfigurationVariables = this.MwsConfigurationVariables.Clone(), 
                MwsCommonData = this.MwsCommonData.Clone(), 
                MwsProdSettingsClass = this.MwsProdSettingsClass.Clone(),
                MwsUserSettings = this.MwsUserSettings.Clone(), 
                MwsTable = this.MwsTable.Clone(),
                MwsRealTimeData = this.MwsRealTimeData.Clone() 
            };
        }


    }
}
