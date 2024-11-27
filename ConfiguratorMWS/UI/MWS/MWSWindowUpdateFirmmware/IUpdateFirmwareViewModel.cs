

using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware
{
    public interface IUpdateFirmwareViewModel
    {
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand UpdateFirmwareCommand { get; }
        public RelayCommand UpdateFirmwareFromRemoteServerCommand { get; }
        public RelayCommand SetDefaultSettingsCommand { get; }
    }
}
