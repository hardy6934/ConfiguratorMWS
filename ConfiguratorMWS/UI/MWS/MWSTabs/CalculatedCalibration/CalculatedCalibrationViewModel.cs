using ConfiguratorMWS.Base;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSSubModels; 
using Microsoft.Win32; 
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel; 
using System.Windows; 
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Resources;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration
{
    public class CalculatedCalibrationViewModel : ViewModelBase, ICalculatedCalibrationViewModel
    { 
        private readonly ISettingsViewModelService settingsViewModelService;
        public MWSEntity mWSEntity { get; set; }
        public RelayCommand CalculateTableCommand { get; }
        public RelayCommand UploadXlsFileCommand { get; }
        public RelayCommand SaveCalculatedTareToFileCommand { get; }
        public RelayCommand saveSettingsCommand { get; }
        public MwsTableClass calculatedTable;


        public LocalizedStrings localizedStrings { get; set; }

        public CalculatedCalibrationViewModel(IMWSRepository mWSRepository, ISettingsViewModelService settingsViewModelService)
        { 
            this.settingsViewModelService = settingsViewModelService;
            mWSEntity = settingsViewModelService.GetEntity();

            localizedStrings = (LocalizedStrings)Application.Current.Resources["LocalizedStrings"];

            calculatedTable = new MwsTableClass();
            SaveCalculatedTareToFileCommand = new RelayCommand(SaveCalculatedTareToFile, (obj) => CalculatedTable.Rows.Count > 0);
            CalculateTableCommand = new RelayCommand(CalculateCalibrationTable, null);
            UploadXlsFileCommand = new RelayCommand(UploadeXlsFile, null);
            saveSettingsCommand = new RelayCommand(SaveSettingsAsync, (obj) => (mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged) && (CalculatedTable.Rows.Count > 0));
            
            // Подписываемся на изменения свойства CommandStatus
            mWSEntity.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(mWSEntity.CommandStatus))
                {
                    saveSettingsCommand.RaiseCanExecuteChanged();
                }
            };

        }



        public MwsTableClass CalculatedTable
        {
            get => calculatedTable;
            set
            {
                calculatedTable = value;
                RaisePropertyChanged(nameof(CalculatedTable)); 
            }
        }
         
        private double cylindricalPart;
        public double CylindricalPart
        {
            get => cylindricalPart;
            set
            {
                if (value >= 0)
                {
                    cylindricalPart = value;
                    RaisePropertyChanged(nameof(CylindricalPart)); 
                }
            }
        }
         

        private double diameter;  
        public double Diameter
        {
            get { 
                return diameter;
            }
            set
            {
                if (value >= 0)
                {
                    diameter = value;
                    RaisePropertyChanged(nameof(Diameter));
                }
            }
        }   


        private double elipticalSideWall;
        public double ElipticalSideWall
        {
            get 
            { 
                return elipticalSideWall;
            }
            set
            {
                if (value >= 0)
                {
                    elipticalSideWall = value;
                    RaisePropertyChanged(nameof(ElipticalSideWall));
                }
            }
        }

        private double sensorsHeight;
        public double SensorsHeight
        {
            get => sensorsHeight;
            set
            {
                if (value >= 0)
                {
                    sensorsHeight = value;
                    RaisePropertyChanged(nameof(SensorsHeight));
                }
            }
        }
         


        public async Task SaveSettingsAsync(object obj)
        {
            settingsViewModelService.ChangeProgressBarValue(0);// progress bar
            Array.Copy(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead, 0x800, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x800, 0x40);

            var mwsSettingsForSendingStruct = new mwsSettingsForSending();
            mwsSettingsForSendingStruct.userSettings = settingsViewModelService.ConvertUserSettingsClassToStruct(mWSEntity.MwsUserSettings);

            var table = CalculatedTable.Clone();
            foreach (var row in table.Rows) {
                row.distance = row.distance;
            }
            mWSEntity.MwsTable = table;
            mwsSettingsForSendingStruct.table = settingsViewModelService.ConvertMwsTableClassToStruct(mWSEntity.MwsTable);

            mwsSettingsForSendingStruct.userSettings.flashUserpassword = 0xAC09;

            mwsSettingsForSendingStruct.userSettings.flashUserSetDay = (byte)DateTime.Now.Day;
            mwsSettingsForSendingStruct.userSettings.flashUserSetMonth = (byte)DateTime.Now.Month;
            mwsSettingsForSendingStruct.userSettings.flashUserSetYear = (byte)DateTime.Now.Year;


            byte[] arrayUserSettings = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.userSettings);
            Array.Copy(arrayUserSettings, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x800, arrayUserSettings.Length);

            byte[] arrayTable = settingsViewModelService.RawSerialize(mwsSettingsForSendingStruct.table);
            Array.Copy(arrayTable, 0, mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, 0x1000, (mWSEntity.MwsTable.Rows.Count() + 1) * 8);


            settingsViewModelService.ChangeTimerWorkInterval(100);
            
            mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800;
            mWSEntity.MwsConfigurationVariables.ConfirmAddress = 10000;

            await settingsViewModelService.SendSettingsOnServerAsync(mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr, mWSEntity.MwsCommonData.SerialNumberFullFormat, mWSEntity.MwsCommonData.SensorTypeForDisplaing);

            mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceFlashClear;

        }


        public void CalculateCalibrationTable(object obj)
        {
            

            if (CylindricalPart != 0 && SensorsHeight != 0 && Diameter != 0)
            {
                calculatedTable.Rows.Clear();

                double incLenght = Diameter / 39;
                double radius = Diameter / 2;
                double heightSensorKoef = SensorsHeight - Diameter;

                double volume;
                double level;
                double aplh;

                for (int i = 0; i < 40; i++)
                {
                    level = incLenght * i;

                    if (level < radius)
                    {
                        aplh = 2 * Math.Acos((radius - level) / radius);
                        volume = radius * radius / 2 * (aplh - Math.Sin(aplh));
                    }
                    else
                    {
                        aplh = 2 * Math.Acos((level - radius) / radius);
                        volume = Math.PI * radius * radius - radius * radius / 2 * (aplh - Math.Sin(aplh));
                    }

                    volume = volume * CylindricalPart + (Math.PI * ((2 * ElipticalSideWall) / Diameter) * (((Diameter * level * level) / 2) - ((level * level * level) / 3)));

                    calculatedTable.Rows.Add(new MwsRowClass { Number = i + 1, Distance = Convert.ToSingle(Math.Round(SensorsHeight - level)), Volume = Convert.ToSingle(Math.Round(volume / 1000000, 0, MidpointRounding.AwayFromZero)) });
                }
            }
            else
            {
                MessageBox.Show(localizedStrings["NotAllFieldsFilledIn"], localizedStrings["strError"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void UploadeXlsFile(object obj)
        {
            // Открываем диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (fileExtension == ".csv")
                {
                    ReadCsvFile(filePath);
                }
                else if (fileExtension == ".xls")
                {
                    ReadXlsFile(filePath);
                }
                else if (fileExtension == ".xlsx")
                {
                    ReadXlsxFile(filePath);
                }
            }
        }
        private void ReadCsvFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    calculatedTable.Rows.Clear();
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        if (values.Length >= 2 && float.TryParse(values[0], out float volume) && float.TryParse(values[1], out float distance))
                        {
                            i++;
                            calculatedTable.Rows.Add(new MwsRowClass { Number = i, Volume = volume, Distance = distance });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(localizedStrings["CSVReadingFileError"], localizedStrings["strError"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ReadXlsFile(string filePath)
        {
            try
            {
                calculatedTable.Rows.Clear();

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new HSSFWorkbook(fileStream); // Для .xls
                    var sheet = workbook.GetSheetAt(0); // Используем первый лист
                    int i = 0;

                    for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var row = sheet.GetRow(rowIndex);
                        if (row == null) continue;

                        // Чтение значений Volume и Distance из первых двух столбцов
                        if (float.TryParse(row.GetCell(0)?.ToString(), out float volume) &&
                            float.TryParse(row.GetCell(1)?.ToString(), out float distance))
                        {
                            i++;
                            calculatedTable.Rows.Add(new MwsRowClass { Number = i, Volume = volume, Distance = distance });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(localizedStrings["XLSReadingFileeError"], localizedStrings["strError"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ReadXlsxFile(string filePath)
        {
            try
            {
                calculatedTable.Rows.Clear();

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(fileStream); // Для .xlsx
                    var sheet = workbook.GetSheetAt(0); // Используем первый лист
                    int i = 0;

                    for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var row = sheet.GetRow(rowIndex);
                        if (row == null) continue;

                        // Чтение значений Volume и Distance из первых двух столбцов
                        if (float.TryParse(row.GetCell(0)?.ToString(), out float volume) &&
                            float.TryParse(row.GetCell(1)?.ToString(), out float distance))
                        {
                            i++;
                            calculatedTable.Rows.Add(new MwsRowClass { Number = i, Volume = volume, Distance = distance });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(localizedStrings["XLSXReadingFileeError"], localizedStrings["strError"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        public void SaveCalculatedTareToFile(object obj)
        {
            // Открываем диалоговое окно для выбора места и формата сохранения
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV File (*.csv)|*.csv|Excel 97-2003 (*.xls)|*.xls|Excel File (*.xlsx)|*.xlsx",
                Title = "Сохранить таблицу как..."
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                try
                {
                    if (fileExtension == ".csv")
                    {
                        SaveAsCsv(filePath);
                    }
                    else if (fileExtension == ".xls")
                    {
                        SaveAsXls(filePath);
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        SaveAsXlsx(filePath);
                    }

                    MessageBox.Show(localizedStrings["SuccesfullySavedFile"], localizedStrings["strSuccess"], MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(localizedStrings["SavingFileError"], localizedStrings["strError"], MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveAsCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                // Записываем заголовок
                writer.WriteLine("Volume;Distance");

                // Записываем строки
                foreach (var row in calculatedTable.Rows)
                {
                    writer.WriteLine($"{row.Volume};{row.Distance}");
                }
            }
        }
        private void SaveAsXls(string filePath)
        {
            var workbook = new HSSFWorkbook(); // Для .xls
            var sheet = workbook.CreateSheet("TableData");

            // Добавляем заголовок
            var headerRow = sheet.CreateRow(0); 
            headerRow.CreateCell(0).SetCellValue("Volume");
            headerRow.CreateCell(1).SetCellValue("Distance");

            // Добавляем данные
            for (int i = 0; i < calculatedTable.Rows.Count; i++)
            {
                var dataRow = sheet.CreateRow(i + 1);
                var row = calculatedTable.Rows[i];

                dataRow.CreateCell(0).SetCellValue(row.Volume);
                dataRow.CreateCell(1).SetCellValue(row.Distance);
            }

            // Сохраняем файл
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }
        }
        private void SaveAsXlsx(string filePath)
        {
            var workbook = new XSSFWorkbook(); // Для .xlsx
            var sheet = workbook.CreateSheet("TableData");

            // Добавляем заголовок
            var headerRow = sheet.CreateRow(0); 
            headerRow.CreateCell(0).SetCellValue("Volume");
            headerRow.CreateCell(1).SetCellValue("Distance");

            // Добавляем данные
            for (int i = 0; i < calculatedTable.Rows.Count; i++)
            {
                var dataRow = sheet.CreateRow(i + 1);
                var row = calculatedTable.Rows[i];
                 
                dataRow.CreateCell(0).SetCellValue(row.Volume);
                dataRow.CreateCell(1).SetCellValue(row.Distance);
            }

            // Сохраняем файл
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }
        }



    }
}
