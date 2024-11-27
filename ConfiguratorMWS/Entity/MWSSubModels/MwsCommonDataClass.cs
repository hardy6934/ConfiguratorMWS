

using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsCommonDataClass: ObservableBase
    {

        public uint serialNumber; 
        public byte sensorType;
        public string hardVersion;
        public string softVersion;
        

        public uint SerialNumber
        {
            get => serialNumber;
            set
            {
                serialNumber = value;
                RaisePropertyChanged(nameof(SerialNumber));
                RaisePropertyChanged(nameof(SerialNumberFullFormat));
            }
        }

        public uint SerialNumberFullFormat
        {
            get => serialNumber == 0 ? 0 : serialNumber + 106000000;
            set { 
            }
        }

        public byte SensorType
        {
            get => sensorType;
            set
            {
                sensorType = value;
                RaisePropertyChanged(nameof(SensorType));
                RaisePropertyChanged(nameof(SensorTypeForDisplaing));
            }
        }
        public string SensorTypeForDisplaing
        { 
            get
            {
                switch (sensorType)
                {
                    case 0xAA: return "MWS RS 2";
                    case 0xAB: return "MWS 2";
                    default: return "MWS";
                }
            }
        }

        public string HardVersion
        {
            get => hardVersion;
            set
            {
                hardVersion = value;
                RaisePropertyChanged(nameof(HardVersion));
            }
        }

        public string SoftVersion
        {
            get => softVersion;
            set
            {
                softVersion = value;
                RaisePropertyChanged(nameof(SoftVersion));
            }
        }



        public MwsCommonDataClass Clone()
        {
            return new MwsCommonDataClass
            {
                SerialNumber = this.SerialNumber,
                SensorType = this.SensorType,
                HardVersion = this.HardVersion,
                SoftVersion = this.SoftVersion
            };
        }


    }
}
