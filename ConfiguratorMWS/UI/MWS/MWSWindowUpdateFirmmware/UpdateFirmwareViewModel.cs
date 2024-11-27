using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Resources;
using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Windows; 

namespace ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware
{
    public class UpdateFirmwareViewModel: IUpdateFirmwareViewModel
    {
        private readonly IUpdateFirmwareViewModelService updateFirmwareViewModelService; 
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand UpdateFirmwareCommand { get; }
        public RelayCommand UpdateFirmwareFromRemoteServerCommand { get; }
        public RelayCommand SetDefaultSettingsCommand { get; }
        public LocalizedStrings localizedStrings { get; set; } 

        public UpdateFirmwareViewModel(IUpdateFirmwareViewModelService updateFirmwareViewModelService)
        {
            localizedStrings = (LocalizedStrings)Application.Current.Resources["LocalizedStrings"];
            this.updateFirmwareViewModelService = updateFirmwareViewModelService;
            this.mWSEntity = this.updateFirmwareViewModelService.GetEntity();

            UpdateFirmwareCommand = new RelayCommand(UpdateFirmware, (obj) => mWSEntity.isConnected); 
            UpdateFirmwareFromRemoteServerCommand = new RelayCommand(UpdateFirmwareFromRemoteServerAsync, (obj) => mWSEntity.isConnected);
            SetDefaultSettingsCommand = new RelayCommand(SetDefaultSettings, (obj) => mWSEntity.isConnected);

        }

        private void UpdateFirmware(object obj) {
             
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "cod files (*.cod)|*.cod";
            openFileDialog.Title = "Выберите файл .cod";

            mWSEntity.MwsConfigurationVariables.updateIndexTRX = 0;
            mWSEntity.MwsConfigurationVariables.indexRxDataBoot = 0;
            mWSEntity.MwsConfigurationVariables.updateCountBytesRX = 4;

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                UpdatingFirmware(filePath);
            }


        }

        private void SetDefaultSettings(object obj) {
            mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceSetDefaultSettings;
            updateFirmwareViewModelService.UpdateWindowProgresBarStatus("");
        }


        private async Task UpdateFirmwareFromRemoteServerAsync(object obj) {
            var url = Properties.Settings.Default.ApiUrl;
            var pathForSave = Properties.Settings.Default.FilePath; 

            try {

                using (HttpClient client = new HttpClient())
                {

                    var response = await client.GetAsync(url + "/Ping");
                    if (response.IsSuccessStatusCode)
                    {

                        response = await client.GetAsync(url + "/api/APISoftware?typeName=" + "MWS 2" + "&hardVersion=" + mWSEntity.MwsCommonData.HardVersion + "&softVersion=" + mWSEntity.MwsCommonData.SoftVersion);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            if (int.TryParse(responseContent, out int Id))
                            {
                                var result = MessageBox.Show(localizedStrings["NotLastFirmwsareVer"], localizedStrings["SoftwareUpdate"], MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    response = await client.GetAsync(url + "/api/APISoftware/Id?id=" + Id);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        var fileBytes = await response.Content.ReadAsByteArrayAsync();

                                        var fileName = response.Content.Headers.ContentDisposition.FileName.Trim('"');

                                        string filePath = Path.Combine(pathForSave, fileName);

                                        using (FileStream fstream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await fstream.WriteAsync(fileBytes, 0, fileBytes.Length);
                                        }

                                        if (File.Exists(filePath))
                                        {
                                            UpdatingFirmware(filePath);
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show(localizedStrings["ErrorWhileUpdating"], localizedStrings["strError"]);
                                        return;
                                    }
                                } 
                            } 
                        } 
                    }
                } 
            }
            catch (Exception ex)
            { 
                return;
            }
            

        } 
         

         

        public void UpdatingFirmware(string filePath) { 

            try
            { 
                // Чтение файла как массива байт
                mWSEntity.MwsConfigurationVariables.updateBufferFileCOD = File.ReadAllBytes(filePath);
                if (mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length < 4)
                {
                    MessageBox.Show(localizedStrings["strFileError"], localizedStrings["strError"]);
                    return;
                }

                if (!mWSEntity.MwsConfigurationVariables.deadMode)
                {
                    if (mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length - 4] != mWSEntity.MwsConfigurationVariables.updateTypeDevice)
                    {
                        MessageBox.Show(localizedStrings["strIsNotIntended"], localizedStrings["strError"]);
                        return;
                    }

                    if (mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length - 1] != mWSEntity.MwsConfigurationVariables.updateHardDevice)
                    {
                        MessageBox.Show(localizedStrings["strHardwareVersionNotMatch"], localizedStrings["strError"]);
                        return;
                    }
                }
                else
                {
                    mWSEntity.MwsConfigurationVariables.updateTypeDevice = mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length - 4];
                    mWSEntity.MwsConfigurationVariables.updateHardDevice = mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length - 1];
                }

                mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceUpdateRequestReset;
                updateFirmwareViewModelService.ChangeUpdateProgressBarValue(50);
                updateFirmwareViewModelService.UpdateWindowProgresBarStatus("");
            }
            catch
            {
                MessageBox.Show(localizedStrings["strFileError"], localizedStrings["strError"]);
                return;
            }

        }


    }
}
