

using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity
{
    public class MWSEntity: ObservableBase, SensorModel
    {
        public bool isConnected;
        public uint sensorType;
        public uint serialNumber;
        public string hardVersion;
        public string softVersion;
        public float distance;
        public float level;
        public float volume;
        public short temp;
        public ushort nOmni;
        public uint flags;


        public int CurrentAddress { get; set; } = 0;
        public int ConfirmAddress { get; set; } = 0;
        public int CommandStatus { get; set; } = 0;
        public int CommandLastReadedBytes { get; set; } = 0;
        public int CountFF { get; set; }
        public int Pass { get; set; } = 0xAC09;
        public int Config { get; set; } = 0x05;


        public uint SensorType
        {
            get
            {
                return sensorType;
            }
            set
            {
                if (sensorType != value)
                {
                    sensorType = value;
                    RaisePropertyChanged("SensorType");
                }
            }
        }

        public uint SerialNumber
        {
            get
            {
                return serialNumber;
            }
            set
            {
                if (serialNumber != value)
                {
                    serialNumber = value;
                    RaisePropertyChanged("SerialNumber");
                }
            }
        }

        public string HardVersion
        {
            get
            {
                return hardVersion;
            }
            set
            {
                if (hardVersion != value)
                {
                    hardVersion = value;
                    RaisePropertyChanged("HardVersion");
                }
               
            }
        }

        public string SoftVersion
        {
            get
            {
                return softVersion;
            }
            set
            {
                if (softVersion != value)
                {
                    softVersion = value;
                    RaisePropertyChanged("SoftVersion");
                }
                
            }
        }

        public float Distance 
        {
            get {
                return (float)Math.Round(distance, 2);
            }
            set
            {
                if (distance != value)
                {
                    distance = value;
                    RaisePropertyChanged("Distance");
                }
            }
        }

        public float Level
        {
            get {
                return (float)Math.Round(level, 2);
            }
            set
            {
                if (level != value)
                {
                    level = value;
                    RaisePropertyChanged("Level");
                }
            }
        } 

        public float Volume
        {
            get {
                return (float)Math.Round(volume, 2);
            }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    RaisePropertyChanged("Volume");
                    RaisePropertyChanged("HeightOfFulkelInTheTank");
                }
            }
        } 

        public short Temp
        {
            get {
                return temp;
            }
            set
            {
                if (temp != value)
                {
                    temp = value;
                    RaisePropertyChanged("Temp");
                }
            }
        } 

        public ushort NOmni
        {
            get {
                return nOmni;
            }
            set
            {
                if (nOmni != value)
                {
                    nOmni = value;
                    RaisePropertyChanged("NOmni");
                }
            }
        }

        public uint Flags
        {
            get {
                return flags;
            }
            set
            {
                if (flags != value)
                {
                    flags = value;
                    RaisePropertyChanged("Flags");
                }
            }
        }   




        public bool IsConnected
        {
            get
            {
                return isConnected;
            } 
            set
            {
                isConnected = value;
            } 

        }

        public enum MwsStatuses
        {
            Command71Accepted,
            Command80Accepted,
            Command85Accepted,
            Command90Accepted,
            Command91Accepted,
            Command92Accepted
        }
         
        public double HeightOfFulkelInTheTank
        {
            get {
                return volume * 2 ; 
            }
        }

    }
}
