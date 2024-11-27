

using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsUserSettingsClass: ObservableBase
    {
        public ushort flashUserSetYear {  get; set; }
        public byte flashUserSetMonth { get; set; }
        public byte flashUserSetDay { get; set; }
        public uint flashUserSerialNumber { get; set; }
        public ushort flashUserpassword { get; set; }

        public uint flashUserConfigFlags;
        public uint flashUserBaudRateRs { get; set; }
        public uint flashUserSpeedCan { get; set; }

        public byte flashUserAdrSensor { get; set; }
        public ushort flashUserLlsMinN { get; set; }
        public ushort flashUserLlsMaxN { get; set; }

        public byte flashUserSaCan { get; set; }

        public float flashUserTankHeight { get; set; }
        public ushort flashUserAverageS { get; set; }
        public float flashUserThreshold { get; set; }



        public ushort FlashUserSetYear
        {
            get => flashUserSetYear;
            set
            {
                flashUserSetYear = value;
                RaisePropertyChanged(nameof(FlashUserSetYear)); 
            }
        }

        public byte FlashUserSetMonth
        {
            get => flashUserSetMonth;
            set
            {
                flashUserSetMonth = value;
                RaisePropertyChanged(nameof(FlashUserSetMonth));
            }
        }

        public byte FlashUserSetDay
        {
            get => flashUserSetDay;
            set
            {
                flashUserSetDay = value;
                RaisePropertyChanged(nameof(FlashUserSetDay));
            }
        }
         
        public uint FlashUserSerialNumber
        {
            get => flashUserSerialNumber;
            set
            {
                flashUserSerialNumber = value;
                RaisePropertyChanged(nameof(FlashUserSerialNumber));
            }
        }

        public ushort FlashUserpassword
        {
            get => flashUserpassword;
            set
            {
                flashUserpassword = value;
                RaisePropertyChanged(nameof(FlashUserpassword));
            }
        }

        public uint FlashUserBaudRateRs
        {
            get => flashUserBaudRateRs;
            set
            {
                flashUserBaudRateRs = value;
                RaisePropertyChanged(nameof(FlashUserBaudRateRs));
            }
        }

        public uint FlashUserSpeedCan
        {
            get => flashUserSpeedCan;
            set
            {
                flashUserSpeedCan = value;
                RaisePropertyChanged(nameof(FlashUserSpeedCan)); 
            }
        }

        public byte FlashUserAdrSensor
        {
            get => flashUserAdrSensor;
            set
            {
                flashUserAdrSensor = value;
                RaisePropertyChanged(nameof(FlashUserAdrSensor)); 
            }
        }

        public ushort FlashUserLlsMinN
        {
            get => flashUserLlsMinN;
            set
            {
                flashUserLlsMinN = value;
                RaisePropertyChanged(nameof(FlashUserLlsMinN)); 
            }
        }

        public ushort FlashUserLlsMaxN
        {
            get => flashUserLlsMaxN;
            set
            {
                flashUserLlsMaxN = value;
                RaisePropertyChanged(nameof(FlashUserLlsMaxN)); 
            }
        }

        public byte FlashUserSaCan
        {
            get => flashUserSaCan;
            set
            {
                flashUserSaCan = value;
                RaisePropertyChanged(nameof(FlashUserSaCan));
            }
        }

        public float FlashUserTankHeight
        {
            get => flashUserTankHeight;
            set
            {
                flashUserTankHeight = value;
                RaisePropertyChanged(nameof(FlashUserTankHeight)); 
            }
        }

        public ushort FlashUserAverageS
        {
            get => flashUserAverageS;
            set
            {
                flashUserAverageS = value;
                RaisePropertyChanged(nameof(FlashUserAverageS)); 
            }
        }

        public float FlashUserThreshold
        {
            get => flashUserThreshold;
            set
            {
                flashUserThreshold = value;
                RaisePropertyChanged(nameof(FlashUserThreshold)); 
            }
        }




        public uint FlashUserConfigFlags
        {
            get
            {
                return flashUserConfigFlags;
            }
            set
            {
                flashUserConfigFlags = value;
                RaisePropertyChanged(nameof(FlashUserConfigFlags));
                RaisePropertyChanged(nameof(TogglePodtyazhkaFlag));
                RaisePropertyChanged(nameof(ToggleTerminatorFlag));
            }
        }
        public bool TogglePodtyazhkaFlag
        {
            get
            {
                if ((flashUserConfigFlags & (1 << 1)) != 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                {
                    flashUserConfigFlags |= 1 << 1;
                }
                else
                {
                    FlashUserConfigFlags &= ~(uint)(1 << 1);
                }
                RaisePropertyChanged(nameof(TogglePodtyazhkaFlag));
            }
        }
        public bool ToggleTerminatorFlag
        {
            get
            {
                if ((flashUserConfigFlags & (1 << 0)) != 0)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                {
                    flashUserConfigFlags |= 1 << 0;
                }
                else
                {
                    FlashUserConfigFlags &= ~(uint)(1 << 0);
                }
                RaisePropertyChanged(nameof(ToggleTerminatorFlag));
            }
        }



        public MwsUserSettingsClass Clone()
        {
            return new MwsUserSettingsClass
            {
                flashUserSetYear = this.flashUserSetYear,
                flashUserSetMonth = this.flashUserSetMonth,
                flashUserSetDay = this.flashUserSetDay,
                flashUserSerialNumber = this.flashUserSerialNumber,
                flashUserpassword = this.flashUserpassword,
                flashUserConfigFlags = this.flashUserConfigFlags,
                flashUserBaudRateRs = this.flashUserBaudRateRs,
                flashUserSpeedCan = this.flashUserSpeedCan,
                flashUserAdrSensor = this.flashUserAdrSensor,
                flashUserLlsMinN = this.flashUserLlsMinN,
                flashUserLlsMaxN = this.flashUserLlsMaxN,
                flashUserSaCan = this.flashUserSaCan,
                flashUserTankHeight = this.flashUserTankHeight,
                flashUserAverageS = this.flashUserAverageS,
                flashUserThreshold = this.flashUserThreshold
            };
        } 

    }
}
