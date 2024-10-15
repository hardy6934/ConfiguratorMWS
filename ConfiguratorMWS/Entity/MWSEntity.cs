

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

        public int CommandStatus { get; set; }
        public int CommandLastReadedBytes { get; set; }
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
                return distance;
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
                return level;
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
                return volume;
            }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    RaisePropertyChanged("Volume");
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

    }
}
