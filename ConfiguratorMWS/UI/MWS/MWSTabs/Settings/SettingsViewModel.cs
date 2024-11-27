#define PROD

using ConfiguratorMWS.Base;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using Microsoft.Win32;
using NPOI.Util;
using System.IO;
using System.Runtime.InteropServices;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Settings
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ISettingsViewModelService settingsViewModelService; 
        private readonly IInformationViewModelService informationViewModelService; 
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand saveSettings { get;}
        public RelayCommand saveSettingsToFileCommand { get;}
        public RelayCommand readSettingsFromFileCommand { get;}
         

        public SettingsViewModel(ISettingsViewModelService settingsViewModelService, IInformationViewModelService informationViewModelService)
        {
            this.settingsViewModelService = settingsViewModelService;
            this.informationViewModelService = informationViewModelService;

            mWSEntity = settingsViewModelService.GetEntity();

            saveSettings = new RelayCommand(SaveSettings, (obj) => mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged);
            saveSettingsToFileCommand = new RelayCommand(SaveBytesToTextFile, (obj) => mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged);
            readSettingsFromFileCommand = new RelayCommand(ReadBytesFromTextFile, null);

            // Подписываемся на изменения свойства CommandStatus
            mWSEntity.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(mWSEntity.CommandStatus))
                {
                    saveSettings.RaiseCanExecuteChanged();
                }
            }; 
        }

        public bool IsProd { get; } =
#if PROD
            true;
#else
            false;
#endif
        
        private string prodType;
        public string ProdType
        {
            get
            {
                if (mWSEntity.MwsProdSettingsClass.DeviceType == 0xAA && string.IsNullOrEmpty(prodType))
                {
                    prodType = "MWS RS 2"; 
                }
                else if (mWSEntity.MwsProdSettingsClass.DeviceType == 0xAB && string.IsNullOrEmpty(prodType))
                {
                    prodType = "MWS 2";
                } 
                return prodType;
            }
            set
            {
                prodType = value;
                RaisePropertyChanged(nameof(ProdType));
            }
        }


        



        public void SaveSettings(object obj)
        {
            settingsViewModelService.ChangeProgressBarValue(500);// progress bar

            var mwsSettingsForSendingStruct = new mwsSettingsForSending();
            mwsSettingsForSendingStruct.userSettings = settingsViewModelService.ConvertUserSettingsClassToStruct(mWSEntity.MwsUserSettings);
            mwsSettingsForSendingStruct.table = settingsViewModelService.ConvertMwsTableClassToStruct(mWSEntity.MwsTable);
            mwsSettingsForSendingStruct.prodSettings = settingsViewModelService.ConvertMwsProdSettingsClassToStruct(mWSEntity.MwsProdSettingsClass);

            mwsSettingsForSendingStruct.userSettings.flashUserSetDay = (byte)DateTime.Now.Day;
            mwsSettingsForSendingStruct.userSettings.flashUserSetMonth = (byte)DateTime.Now.Month;
            mwsSettingsForSendingStruct.userSettings.flashUserSetYear = (byte)DateTime.Now.Year;
            mwsSettingsForSendingStruct.userSettings.flashUserpassword = 0xAC09;

#if PROD

            mwsSettingsForSendingStruct.prodSettings.prodDay = (byte)DateTime.Now.Day;
            mwsSettingsForSendingStruct.prodSettings.prodMonth = (byte)DateTime.Now.Month;
            mwsSettingsForSendingStruct.prodSettings.prodYear = (byte)DateTime.Now.Year;
            mwsSettingsForSendingStruct.prodSettings.password = 0x1223;
             
            mwsSettingsForSendingStruct.prodSettings.serialNumber = mWSEntity.MwsProdSettingsClass.SerialNumber;
            mwsSettingsForSendingStruct.userSettings.flashUserSerialNumber = mWSEntity.MwsProdSettingsClass.SerialNumber;
             
            if (ProdType == "MWS RS 2")
                mwsSettingsForSendingStruct.prodSettings.deviceType = 0xAA;
            else if (ProdType == "MWS 2")
                mwsSettingsForSendingStruct.prodSettings.deviceType = 0xAB;
              
            byte[] arrayProdSettings = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.prodSettings);
            Array.Copy(arrayProdSettings, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0, arrayProdSettings.Length); 
#endif

            byte[] arrayTable = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.table);
            Array.Copy(arrayTable, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x1000, (mWSEntity.MwsTable.Rows.Count() + 1) * 8);


            byte[] arrayUserSettings = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.userSettings);
            Array.Copy(arrayUserSettings, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x800, arrayUserSettings.Length);



            settingsViewModelService.ChangeTimerWorkInterval(100); 
#if PROD
            mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x000;
#else
            mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800; 
#endif 
            mWSEntity.MwsConfigurationVariables.ConfirmAddress = 10000;


            mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceFlashClear; 

        }



        public void SaveBytesToTextFile(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt", 
                DefaultExt = "txt" 
            };
             
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;
                     
                    string base64String = Convert.ToBase64String(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead);
                     
                    File.WriteAllText(filePath, base64String);

                    Console.WriteLine("Массив байт успешно сохранен в файл: " + filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при сохранении файла: " + ex.Message);
                }
            }
        }


        public void ReadBytesFromTextFile(object obj)
        { 

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt", 
                DefaultExt = "txt",                 
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;

                    string base64String = File.ReadAllText(filePath);

                    var readedByteArray = Convert.FromBase64String(base64String);

                    mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead = readedByteArray;
                    ///Mashaling user settings
                    byte[] usersSettingsData = new byte[Marshal.SizeOf<MwsUserSettings>()];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead, 0x800, usersSettingsData, 0, usersSettingsData.Length);
                    MwsUserSettings userSettingsStruct = informationViewModelService.ReadStruct<MwsUserSettings>(usersSettingsData);
                    MwsUserSettingsClass mwsUsesrSettingsData = informationViewModelService.LoadUsersSettingsFromStruct(userSettingsStruct);
                    if (mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged)
                    {
                        mwsUsesrSettingsData.flashUserSerialNumber = mWSEntity.MwsUserSettings.flashUserSerialNumber;
                        mWSEntity.MwsUserSettings = mwsUsesrSettingsData.Copy();
                    }
                    else {
                        mWSEntity.MwsUserSettings = mwsUsesrSettingsData.Copy();
                    }
                    ///Mashaling urser settings


                    ///Mashaling calibr table
                    byte[] calibrData = new byte[Marshal.SizeOf<MwsTable>()];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead, 0x1000, calibrData, 0, calibrData.Length);
                    MwsTable mwsTableStruct = informationViewModelService.ReadStruct<MwsTable>(calibrData);
                    MwsTableClass mwsTableData = informationViewModelService.LoadCalibrTableFromStruct(mwsTableStruct);
                    mWSEntity.MwsTable = mwsTableData;
                    ///Mashaling

                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.Command90Accepted;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при чтении файла: " + ex.Message);
                }
            } 
        }


    }
}