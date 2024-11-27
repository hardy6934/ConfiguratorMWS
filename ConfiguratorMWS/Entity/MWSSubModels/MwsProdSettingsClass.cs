
using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsProdSettingsClass: ObservableBase
    {
        private ushort prodYear;
        private byte prodMonth;
        private byte prodDay;
        private uint serialNumber;
        private byte deviceType;
        private ushort password;

        public ushort ProdYear
        {
            get => prodYear;
            set
            {
                prodYear = value;
                RaisePropertyChanged(nameof(ProdYear));
            }
        }

        public byte ProdMonth
        {
            get => prodMonth;
            set
            {
                prodMonth = value;
                RaisePropertyChanged(nameof(ProdMonth));
            }
        }

        public byte ProdDay
        {
            get => prodDay;
            set
            {
                prodDay = value;
                RaisePropertyChanged(nameof(ProdDay));
            }
        }

        public uint SerialNumber
        {
            get => serialNumber;
            set
            {
                serialNumber = value;
                RaisePropertyChanged(nameof(SerialNumber));
            }
        }

        public byte DeviceType
        {
            get => deviceType;
            set
            {
                deviceType = value;
                RaisePropertyChanged(nameof(DeviceType));
            }
        }

        public ushort Password
        {
            get => password;
            set
            {
                password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }


        public MwsProdSettingsClass Clone()
        {
            return new MwsProdSettingsClass
            {
                ProdYear = this.ProdYear,
                ProdMonth = this.ProdMonth,
                ProdDay = this.ProdDay,
                SerialNumber = this.SerialNumber,
                DeviceType = this.DeviceType,
                Password = this.Password
            };
        }

    }
}
