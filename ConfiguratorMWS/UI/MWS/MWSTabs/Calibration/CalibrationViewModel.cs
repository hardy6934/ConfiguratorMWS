using ConfiguratorMWS.Base;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using Microsoft.Win32;
using NPOI.Util;
using System.IO;
using System.Runtime.InteropServices;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Calibration
{
    public class CalibrationViewModel : ViewModelBase, ICalibrationViewModel
    {
        private readonly IMWSRepository mWSRepository;
        private readonly ISettingsViewModelService settingsViewModelService; 
        private readonly IInformationViewModelService informationViewModelService;
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand saveSettings { get; }
        public RelayCommand addRowInTable { get; }
        public RelayCommand removeRowFromTable { get; }
        public RelayCommand saveSettingsToFileCommand { get; }
        public RelayCommand readSettingsFromFileCommand { get; }
        public float textBoxPortion { get; set; } = 10;
        public float TextBoxPortion
        {
            get => textBoxPortion;
            set
            {
                textBoxPortion = value;
                RaisePropertyChanged(nameof(TextBoxPortion));
            }
        }

        private MwsRowClass selectedRow;
        public MwsRowClass SelectedRow
        {
            get => selectedRow;
            set
            {
                selectedRow = value;
                RaisePropertyChanged(nameof(SelectedRow)); 
            }
        }

        public CalibrationViewModel(IMWSRepository mWSRepository, ISettingsViewModelService settingsViewModelService, IInformationViewModelService informationViewModelService)
        {
            this.mWSRepository = mWSRepository;
            this.settingsViewModelService = settingsViewModelService;
            this.informationViewModelService = informationViewModelService;
            mWSEntity = mWSRepository.GetEntity();

            saveSettings = new RelayCommand(SaveSettings, (obj) => mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged);
            addRowInTable = new RelayCommand(AddRowInTable, null);
            removeRowFromTable = new RelayCommand(RemoveRowFromTable, (obj) => SelectedRow != null);
            saveSettingsToFileCommand = new RelayCommand(SaveBytesToTextFile, (obj) => mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged);
            readSettingsFromFileCommand = new RelayCommand(ReadBytesFromTextFile, null);
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


            byte[] arrayTable = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.table);
            Array.Copy(arrayTable, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x1000, (mWSEntity.MwsTable.Rows.Count() + 1) * 8);

            byte[] arrayUserSettings = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.userSettings);
            Array.Copy(arrayUserSettings, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x800, arrayUserSettings.Length);


            settingsViewModelService.ChangeTimerWorkInterval(100);

            mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800;
            mWSEntity.MwsConfigurationVariables.ConfirmAddress = 10000;

            mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceFlashClear;

        } 

        public void AddRowInTable(object obj)
        {
            try
            {
                var lastRow = mWSEntity.MwsTable.Rows.LastOrDefault(); 
                mWSEntity.MwsTable.Rows.Add(new MwsRowClass { Number = (lastRow?.Number ?? 0)  + 1, Distance = mWSEntity.MwsRealTimeData.Distance, Volume = (lastRow?.Volume ?? 0) + TextBoxPortion });
            }
            catch (Exception ex) { 
            }
        } 

        public void RemoveRowFromTable(object obj)
        {
            if (SelectedRow != null)
            {
                mWSEntity.MwsTable.Rows.Remove(SelectedRow);
                SelectedRow = null;

                for (int i = 0; i < mWSEntity.MwsTable.Rows.Count; i++)
                {
                    mWSEntity.MwsTable.Rows[i].Number = i + 1; 
                }

            }
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
                    else
                    {
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
