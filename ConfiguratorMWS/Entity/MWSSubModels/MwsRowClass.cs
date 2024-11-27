

using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsRowClass: ObservableBase
    {
        public float number { get; set; }
        public float distance { get; set; }
        public float volume { get; set; }

        public float Number
        {
            get => number;
            set { 
                number = value;
                RaisePropertyChanged(nameof(Number));
            }
        }
        public float Distance
        {
            get => distance;
            set {
                distance = value;
                RaisePropertyChanged(nameof(Distance));
            }
        }
        public float Volume
        {
            get => volume;
            set {
                volume = value;
                RaisePropertyChanged(nameof(Volume)); 
            }
        }



        // Метод клонирования
        public MwsRowClass Clone()
        {
            return new MwsRowClass
            {
                Number = this.Number,
                Distance = this.Distance,
                Volume = this.Volume
            };
        }


    }
}
