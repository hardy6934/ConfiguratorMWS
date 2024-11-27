using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSSubModels;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration
{
    public interface ICalculatedCalibrationViewModel
    {
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand CalculateTableCommand { get; }
        public RelayCommand UploadXlsFileCommand { get; }
        public RelayCommand SaveCalculatedTareToFileCommand { get; }
        public RelayCommand saveSettingsCommand { get; }
        public MwsTableClass CalculatedTable { get; set; }
    }
}
