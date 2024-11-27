

using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsRealTimeDataClass: ObservableBase
    {
        public float distance;
        public float level;
        public float volume;
        public short temp;
        public ushort n;
        public uint flags;



        public float Distance
        {
            get => (float)Math.Round(distance,2);
            set
            {
                distance = value;
                RaisePropertyChanged(nameof(Distance));
            }
        }

        public float Level
        {
            get => (float)Math.Round(level, 2);
            set
            {
                level = value;
                RaisePropertyChanged(nameof(Level));
            }
        }

        public float Volume
        {
            get => (float)Math.Round(volume, 2);
            set
            {
                volume = value;
                RaisePropertyChanged(nameof(Volume));
            }
        }

        public short Temp
        {
            get => temp;
            set
            {
                temp = value;
                RaisePropertyChanged(nameof(Temp));
            }
        }

        public ushort N
        {
            get => n;
            set
            {
                n = value;
                RaisePropertyChanged(nameof(N));
            }
        }

        public uint Flags
        {
            get => flags;
            set
            {
                flags = value;
                RaisePropertyChanged(nameof(Flags));
            }
        }

         
        public MwsRealTimeDataClass Clone()
        {
            return new MwsRealTimeDataClass
            {
                Distance = this.Distance,
                Level = this.Level,
                Volume = this.Volume,
                Temp = this.Temp,
                N = this.N,
                Flags = this.Flags
            };
        }

    }
}

