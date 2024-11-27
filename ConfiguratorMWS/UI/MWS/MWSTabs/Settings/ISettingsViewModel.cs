

using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Settings
{
    public interface ISettingsViewModel
    {
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand saveSettings { get; }
        public RelayCommand saveSettingsToFileCommand { get; }
        public RelayCommand readSettingsFromFileCommand { get; }

        public bool IsProd { get; }
        public string ProdType { get; set; }

    }
}
