using ConfiguratorMWS.Base;
using ConfiguratorMWS.UI.MWS.MWSTabs.Information;
using ConfiguratorMWS.UI.MWS.MWSTabs.Settings;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.UI.MWS.MWSTabs.Calibration;
using ConfiguratorMWS.UI.MWS.MWSTabs.CalculatedCalibration;
using Microsoft.Extensions.DependencyInjection;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using System.IO.Ports;
using ConfiguratorMWS.Entity;
using System.Windows;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    public class MWSViewModel : ViewModelBase, IMWSViewModel
    {

        private object currentView;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMWSRepository mWSRepository;

        public List<string> PortList { get; set; }
        public string? selectedPort;
        public string isConnectedButtonText = "Connect";

        // Команда для переключения вкладок
        public RelayCommand SwitchTabCommand { get; }
        public RelayCommand connectToPort { get; }
         

        //entity
        private MWSEntity mWSEntity;
         
        //byte array for write
        byte[] bufferFlashDataForWr = new byte[6144];
        // приемные и передающие буферы
        byte[] bufferRxData = new byte[64];

        int indexRxDataInt = 0;
        byte[] bufferRxDataInt = new byte[128];

        int indexRxDataBoot = 0;
        byte[] bufferRxDataBoot = new byte[64];

        byte[] bufferTxData = new byte[64];



       
        public MWSViewModel(IServiceProvider serviceProvider, IMWSService mWSService, IMWSRepository mWSRepository, MWSEntity mWSEntity)
        {
            _serviceProvider = serviceProvider;
            this.mWSRepository = mWSRepository;
            this.mWSEntity = mWSEntity;

            //////////////////////////////////////////////////////////////////
            PortList = mWSRepository.GetAvailableComPortNames();
            selectedPort = PortList.FirstOrDefault(); // Задаем начально выбранный элемент
            //////////////////////////////////////////////////////////////////
             
            mWSRepository.TimerWork(Timer_Tick);

            // Инициализация команды
            SwitchTabCommand = new RelayCommand(SwitchTab); 

            // Устанавливаем начальное содержимое
            CurrentView = new InformationView(_serviceProvider.GetRequiredService<IInformationViewModel>());

            connectToPort = new RelayCommand(ExecuteConnectOrDisconnect, (obj) => !string.IsNullOrEmpty(selectedPort));
        }
         

        ///////////////////////////Методы для работы с UI

        // Метод для обновления списка портов
        public void RefreshComPorts()
        {
            PortList = mWSRepository.GetAvailableComPortNames();
            RaisePropertyChanged("PortList"); 
        }


        public string SelectedPort
        {
            get => selectedPort;
            set
            { 
                selectedPort = value;
                RaisePropertyChanged("SelectedPort");
                 
            }
        }

        public string IsConnectedButtonText
        {
            get => isConnectedButtonText;
            set
            { 
                isConnectedButtonText = value;
                RaisePropertyChanged("IsConnectedButtonText"); 
            }
        }
        
                  
        // Текущее содержимое, которое отображается в нижней части окна
        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                RaisePropertyChanged("CurrentView");
            }
        }
         

        // Логика переключения вкладок
        public void SwitchTab(object tab)
        {
            switch (tab)
            {
                case "Information":
                    CurrentView = new InformationView(_serviceProvider.GetRequiredService<IInformationViewModel>());
                    break;
                case "Settings":
                    CurrentView = new SettingsView(_serviceProvider.GetRequiredService<ISettingsViewModel>());
                    break;
                case "Calibration":
                    CurrentView = new CalibrationView(_serviceProvider.GetRequiredService<ICalibrationViewModel>());
                    break;
                case "CalculatedCalibration":
                    CurrentView = new CalculatedCalibrationView(_serviceProvider.GetRequiredService<ICalculatedCalibrationViewModel>());
                    break;
            }
        }






        ////////////////////////МЕТОДЫ РАБОТЫ С КОМ ПОРТАМИ И ДАННМИ 
        private void ExecuteConnectOrDisconnect(object parameter)
        {
            if (mWSEntity.IsConnected)
            {
                CloseConnectionWithPort(parameter);
            }
            else
            {
                ConnectToPort(parameter);
            }
        }
        public void ConnectToPort(object tab)
        {
            if (mWSRepository.IsPortAvailable(selectedPort)) { 
                var isConnected = mWSRepository.ConnectWithComPort(selectedPort, 19200, ReadBytesFromComPortCallback);
                if (isConnected)
                {
                    mWSRepository.SetIsConnectedTrue();
                    if (mWSEntity.isConnected)
                    {
                        IsConnectedButtonText = "Disconnect";
                    }

                } 
            }else MessageBox.Show("Не удается выполнить подключение к данному порту");

        } 
        public void CloseConnectionWithPort(object tab)
        {
            var isDisconnected = mWSRepository.CloseConnectionWithComPort();
            if (isDisconnected)
            {
                mWSRepository.SetIsConnectedFalse();
                if (!mWSEntity.isConnected) {
                    IsConnectedButtonText = "Connect";
                }

            } 
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            byte[] bufferTxData;
            byte num = 32;

            switch (mWSEntity.CommandStatus) {
                case 0:
                    try
                    {
                        bufferTxData = new byte[4];
                        bufferTxData[0] = 0x44;
                        bufferTxData[1] = 0x1;
                        bufferTxData[2] = 0x80;
                        bufferTxData[3] = CalcCRC(bufferTxData, 3);

                        mWSRepository.WriteData(bufferTxData, 4);
                    }
                    catch(TimeoutException) {
                        CloseConnectionWithPort(null);
                        MessageBox.Show("Вероятнее всего вы подключились к неверному порту"); 
                    } 
                    catch(Exception) {
                        MessageBox.Show("Произошла ошибка записи в порт");
                    } 
                    break;

                case (int)MWSEntity.MwsStatuses.Command80Accepted:
                    bufferTxData = new byte[11];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 8;
                    bufferTxData[2] = 0x85;
                    bufferTxData[3] = (byte)mWSEntity.SensorType;
                    bufferTxData[4] = (byte)mWSEntity.SerialNumber;
                    bufferTxData[5] = (byte)(mWSEntity.SerialNumber >> 8);
                    bufferTxData[6] = (byte)(mWSEntity.SerialNumber >> 16);
                    bufferTxData[7] = (byte)mWSEntity.Config;
                    bufferTxData[8] = (byte)mWSEntity.Pass;
                    bufferTxData[9] = (byte)(mWSEntity.Pass >> 8);
                    bufferTxData[10] = CalcCRC(bufferTxData, bufferTxData[1] + 2);
                      
                    mWSRepository.WriteData(bufferTxData, 11); 
                    break;

                //case (int)MWSEntity.MwsStatuses.Command85Accepted: 
                //    bufferTxData[0] = 0x44;
                //    bufferTxData[1] = 4;
                //    bufferTxData[2] = 0x90;
                //    bufferTxData[3] = (byte)mWSEntity.CommandLastReadedBytes;
                //    bufferTxData[4] = (byte)(mWSEntity.CommandLastReadedBytes >> 8);
                //    bufferTxData[5] = num;
                //    bufferTxData[6] = CalcCRC(bufferTxData, bufferTxData[1] + 2);

                //    mWSRepository.WriteData(bufferTxData, 7); 
                //    break;

                //case (int)MWSEntity.MwsStatuses.Command90Accepted:
                //    bufferTxData[0] = 0x44;
                //    bufferTxData[1] = 3;
                //    bufferTxData[2] = 0x91;
                //    bufferTxData[3] = (byte)mWSEntity.CommandLastReadedBytes;
                //    bufferTxData[4] = (byte)(mWSEntity.CommandLastReadedBytes >> 8);
                //    bufferTxData[5] = CalcCRC(bufferTxData, bufferTxData[1] + 2);

                //    mWSRepository.WriteData(bufferTxData, 7); 
                //    break;

                //case (int)MWSEntity.MwsStatuses.Command91Accepted:
                //    bufferTxData[0] = 0x44;
                //    bufferTxData[1] = (byte)(4 + num);
                //    bufferTxData[2] = 0x92;
                //    bufferTxData[3] = (byte)mWSEntity.CommandLastReadedBytes;
                //    bufferTxData[4] = (byte)(mWSEntity.CommandLastReadedBytes >> 8);
                //    bufferTxData[5] = num;

                //    mWSEntity.CountFF = 0;

                //    for (int i = 0; i < num; i++)
                //    {
                //        bufferTxData[i + 6] = bufferFlashDataForWr[i + mWSEntity.CommandLastReadedBytes];

                //        if (bufferTxData[i + 6] == 0xFF)
                //            mWSEntity.CountFF++;
                //    }
                //    bufferTxData[num + 6] = CalcCRC(bufferTxData, bufferTxData[1] + 2);

                //    mWSRepository.WriteData(bufferTxData, 7); 
                //    break;
 
                case (int)MWSEntity.MwsStatuses.Command85Accepted:
                    bufferTxData = new byte[11];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 0x1;
                    bufferTxData[2] = 0x71;
                    bufferTxData[3] = CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    mWSRepository.WriteData(bufferTxData, 4);
                    break;
            }

        }

        

        public void ReadBytesFromComPortCallback(object sender, SerialDataReceivedEventArgs e)
        { 

            try
            {
                 
                int numRxByte = (sender as SerialPort).Read(bufferRxData, 0, 64);

                Array.Copy(bufferRxData, 0, bufferRxDataInt, indexRxDataInt, numRxByte); 

                if ((indexRxDataBoot + numRxByte) > 64)
                    indexRxDataBoot = 0;

                Array.Copy(bufferRxData, 0, bufferRxDataBoot, indexRxDataBoot, numRxByte);

                indexRxDataInt += numRxByte; 
                indexRxDataBoot += numRxByte;


                do
                {
                    if (indexRxDataInt < 4)
                        break;

                    if (((bufferRxDataInt[0] != 0x45) && (bufferRxDataInt[0] != 0x44))
                        || (bufferRxDataInt[1] > 125))
                    {
                        DeleteRxBytes(ref bufferRxDataInt, 1, ref indexRxDataInt);

                        continue;
                    }

                    if (indexRxDataInt < (bufferRxDataInt[1] + 3))
                        break;

                    if (bufferRxDataInt[bufferRxDataInt[1] + 2] != CalcCRC(bufferRxDataInt, bufferRxDataInt[1] + 2))
                    {
                        DeleteRxBytes(ref bufferRxDataInt, 1, ref indexRxDataInt);
                        continue;
                    }

                    if (bufferRxDataInt[0] == 0x45)
                    {
                        mWSRepository.DecodeIntResponse(bufferRxDataInt);
                    } 

                    DeleteRxBytes(ref bufferRxDataInt, bufferRxDataInt[1] + 3, ref indexRxDataInt);
                }
                while (true);

                //if ((indexRxDataBoot >= updateCountBytesRX) && (updateCountBytesRX != 0))
                //{
                //    mWSRepository.DecodeBootloader(bufferRxDataInt);
                //}

            }
            catch (Exception ex)
            {
                return;
            }

        }


        void DeleteRxBytes(ref byte[] buff, int num, ref int index)
        {
            for (int i = num; i < index; i++)
            {
                buff[i - num] = buff[i];
            }

            index -= num;
        }

      


        byte CalcCRC(byte[] data, int len)
        {
            byte i, j;
            byte crc = 0;
            byte in_bait;

            for (j = 0; j < len; j++)
            {
                in_bait = data[j];

                for (i = 0; i < 8; i++)
                {
                    if (((in_bait & 0x01) ^ (crc & 0x01)) != 0)
                    {
                        crc = (byte)(crc ^ 0x18);
                        crc = (byte)(crc >> 1);
                        crc += 0x80;
                    }
                    else
                    {
                        crc = (byte)(crc >> 1);
                    }

                    in_bait = (byte)(in_bait >> 1);
                }
            }

            return crc;
        }




    }
}
