

using ConfiguratorMWS.Base;

namespace ConfiguratorMWS.Entity
{
    public class TabsSource : ObservableBase
    {
        private string _tab;

        public string Tab
        {
            get { return _tab; }
            set
            {
                if (_tab != value)
                {
                    _tab = value;
                    RaisePropertyChanged("Tab");
                }
            }
        }
    }
}
