#define PROD

using ConfiguratorMWS.Base;
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Commands;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using ConfiguratorMWS.Resources;
using NPOI.Util;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Information
{
    public class InformationViewModel : ViewModelBase, IInformationViewModel
    { 
        private readonly IInformationViewModelService informmationViewModelService; 
        public MWSEntity mWSEntity { get; set; }

        public RelayCommand connectToPort { get; }
        public List<string> PortList { get; set; }
        public string? selectedPort;

        public LocalizedStrings localizedStrings { get; set; }


        public InformationViewModel(IInformationViewModelService informmationViewModelService)
        {
            localizedStrings = (LocalizedStrings)Application.Current.Resources["LocalizedStrings"];
            this.informmationViewModelService = informmationViewModelService;

            mWSEntity = informmationViewModelService.GetEntity();

            informmationViewModelService.TimerWork(Timer_Tick);

            connectToPort = new RelayCommand(ExecuteConnectOrDisconnect, (obj) => !string.IsNullOrEmpty(selectedPort));

            //////////////////////////////////////////////////////////////////
            PortList = informmationViewModelService.GetAvailableComPortNames();
            //////////////////////////////////////////////////////////////////
        }

        //Fuel percentage int tank
        public string topDistanceTank;
        public string topVolumeTank;
        public string bottomDistanceTank;
        public string bottomVolumeTank;
        public double HeightOfFuelInTheTank
        {
            get
            {
                if (mWSEntity.MwsTable.Rows.LastOrDefault() != null && mWSEntity.MwsTable.Rows.LastOrDefault()?.volume != 0)
                {
                    return (mWSEntity.MwsRealTimeData.volume * 200.0) / mWSEntity.MwsTable.Rows.LastOrDefault().volume;
                }
                else
                {
                    return 0;
                }
            } 
        }
        public string TopDistanceTank
        {
            get
            {
                return topDistanceTank;
            }
            set 
            {
                topDistanceTank = value;
                RaisePropertyChanged(nameof(TopDistanceTank));
            }
        }
        public string TopVolumeTank
        {
            get
            {
                return topVolumeTank;
            }
            set 
            {
                topVolumeTank = value;
                RaisePropertyChanged(nameof(TopVolumeTank));
            }
        }
        public string BottomDistanceTank
        {
            get
            {
                return bottomDistanceTank;
            }
            set 
            { 
                bottomDistanceTank = value; 
                RaisePropertyChanged(nameof(BottomDistanceTank));
            }
        }
        public string BottomVolumeTank
        {
            get
            {
               return bottomVolumeTank;
            }
            set
            { 
                bottomVolumeTank = value; 
                RaisePropertyChanged(nameof(bottomVolumeTank));
            }
        }
        //Fuel percentage in tank



        // Метод для обновления списка портов
        public void RefreshComPorts()
        {
            PortList = informmationViewModelService.GetAvailableComPortNames();
            RaisePropertyChanged("PortList");
        }


        public string SelectedPort
        {
            get => selectedPort;
            set
            {
                selectedPort = value;
                RaisePropertyChanged("SelectedPort");
                connectToPort.Execute(value);
            }
        }


        ////////////////////////МЕТОДЫ РАБОТЫ С КОМ ПОРТАМИ
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
            if (informmationViewModelService.IsPortAvailable(tab as string))
            {
                var isConnected = informmationViewModelService.ConnectWithComPort(tab as string, 19200, ReadBytesFromComPortCallback);
                if (isConnected)
                {
                    mWSEntity.IsConnected = true; 
                } 
            }
            else MessageBox.Show("Не удается выполнить подключение к данному порту");

        }
        public void CloseConnectionWithPort(object tab)
        {
            var isDisconnected = informmationViewModelService.CloseConnectionWithComPort();
            if (isDisconnected)
            {
                mWSEntity.IsConnected = false; 
                ConnectToPort(tab as string); 
            }
        }



        ////////////////////////ОБРАБОТКА ДАННЫХ
        private void Timer_Tick(object sender, EventArgs e)
        {
            byte[] bufferTxData;
            byte num = 32;

            if (mWSEntity.CommandStatus == (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged)
            {
                informmationViewModelService.GeneralWindowProgressBarStatus(localizedStrings["Connected"]);
            }
            else {
                informmationViewModelService.GeneralWindowProgressBarStatus("");
            }

            switch (mWSEntity.CommandStatus)
            {
                case 0:
                    try
                    {
                        informmationViewModelService.ChangeProgressBarValue(0);// progress bar
                        informmationViewModelService.ChangeUpdatingProgressBarValue(0);// progress bar update
                        informmationViewModelService.ChangeTimerWorkInterval(50);

                        bufferTxData = new byte[4];
                        bufferTxData[0] = 0x44;
                        bufferTxData[1] = 0x1;
                        bufferTxData[2] = 0x80;
                        bufferTxData[3] = informmationViewModelService.CalcCRC(bufferTxData, 3);

                        informmationViewModelService.WriteData(bufferTxData, 4); 

                    }
                    catch (TimeoutException)
                    {
                        CloseConnectionWithPort(null);
                        MessageBox.Show("Вероятнее всего вы подключились к неверному порту");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Произошла ошибка записи в порт");
                    }
                break;

                case (int)MwsStatusesEnum.Command80Accepted:

                    bufferTxData = new byte[11];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 8;
                    bufferTxData[2] = 0x85;
                    bufferTxData[3] = (byte)mWSEntity.MwsCommonData.SensorType;
                    bufferTxData[4] = (byte)mWSEntity.MwsCommonData.SerialNumber;
                    bufferTxData[5] = (byte)(mWSEntity.MwsCommonData.SerialNumber >> 8);
                    bufferTxData[6] = (byte)(mWSEntity.MwsCommonData.SerialNumber >> 16);
                    bufferTxData[7] = (byte)mWSEntity.MwsConfigurationVariables.Config;
                    bufferTxData[8] = (byte)mWSEntity.MwsConfigurationVariables.Pass;
                    bufferTxData[9] = (byte)(mWSEntity.MwsConfigurationVariables.Pass >> 8);

                    bufferTxData[10] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 11); 
                    break;

                case (int)MwsStatusesEnum.Command85Accepted:
                      
                    bufferTxData = new byte[7];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 4;
                    bufferTxData[2] = 0x90;
                    bufferTxData[3] = (byte)mWSEntity.MwsConfigurationVariables.CurrentAddress;
                    bufferTxData[4] = (byte)(mWSEntity.MwsConfigurationVariables.CurrentAddress >> 8);
                    bufferTxData[5] = num;
                    bufferTxData[6] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 7);
                    break;

                case (int)MwsStatusesEnum.Command90Accepted:

                    informmationViewModelService.ChangeTimerWorkInterval(250);
                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged;
                break;

                case (int)MwsStatusesEnum.Command90AcceptedAndTimerIntervalChanged: 

                    bufferTxData = new byte[11];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 0x1;
                    bufferTxData[2] = 0x71;
                    bufferTxData[3] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 4);

                    if (mWSEntity.MwsConfigurationVariables.CountNotUnansweredCommands71 < 4)
                    {
                        mWSEntity.MwsConfigurationVariables.CountNotUnansweredCommands71++;
                    }
                    else {
                        mWSEntity.CommandStatus = 0;
                    }
                     

                    break;

                case (int)MwsStatusesEnum.DeviceFlashClear:

                    informmationViewModelService.ChangeProgressBarValue(1000);// progress bar

                    bufferTxData = new byte[6];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 3;
                    bufferTxData[2] = 0x91;
                    bufferTxData[3] = (byte)mWSEntity.MwsConfigurationVariables.CurrentAddress;
                    bufferTxData[4] = (byte)(mWSEntity.MwsConfigurationVariables.CurrentAddress >> 8);
                    bufferTxData[5] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 6); 
                break;

                case (int)MwsStatusesEnum.DeviceFlashWrite:
                    informmationViewModelService.ChangeProgressBarValue(3000);// progress bar

                    bufferTxData = new byte[39];

                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = (byte)(4 + num);
                    bufferTxData[2] = 0x92;
                    bufferTxData[3] = (byte)mWSEntity.MwsConfigurationVariables.CurrentAddress;
                    bufferTxData[4] = (byte)(mWSEntity.MwsConfigurationVariables.CurrentAddress >> 8);
                    bufferTxData[5] = (byte)num;
                    mWSEntity.MwsConfigurationVariables.CountFF = 0;

                    for (int i = 0; i < num; i++)
                    {
                        bufferTxData[i + 6] = mWSEntity.MwsConfigurationVariables.bufferFlashDataForWr[i + mWSEntity.MwsConfigurationVariables.CurrentAddress];

                        if (bufferTxData[i + 6] == 0xFF)
                            mWSEntity.MwsConfigurationVariables.CountFF++;
                    }
                    bufferTxData[num + 6] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, (int)(7 + num));
                     
                break;

                case (int)MwsStatusesEnum.DeviceReset:
                    //сброс после загрузки настроек 
                    bufferTxData = new byte[4];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 1;
                    bufferTxData[2] = 0xFF;
                    bufferTxData[3] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 4);

                    informmationViewModelService.ChangeProgressBarValue(6144);// progress bar  
                    mWSEntity.CommandStatus = 0;
                    informmationViewModelService.ChangeTimerWorkInterval(1000);

                break;




                case (int)MwsStatusesEnum.DeviceUpdateRequestReset:
                    //Сброс для обновления ПО
                    bufferTxData = new byte[4];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 1;
                    bufferTxData[2] = 0xFF;
                    bufferTxData[3] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 4);
                     
                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceUpdateRequestBootInfo;
                    informmationViewModelService.ChangeTimerWorkInterval(150);
                break;

                case (int)MwsStatusesEnum.DeviceUpdateRequestBootInfo: 
                    bufferTxData = new byte[2];
                    bufferTxData[0] = 0x5A;
                    bufferTxData[1] = 0x5A;

                    mWSEntity.MwsConfigurationVariables.updateIndexTRX = 0;
                    mWSEntity.MwsConfigurationVariables.indexRxDataBoot = 0;
                    mWSEntity.MwsConfigurationVariables.updateCountBytesRX = 4;

                    //чистка сериал порта
                    informmationViewModelService.DiscardInBuffer();
                    informmationViewModelService.DiscardOutBuffer();

                    informmationViewModelService.WriteData(bufferTxData, 2);
                      
                    informmationViewModelService.ChangeTimerWorkInterval(300); 
                break;

                case (int)MwsStatusesEnum.DviceUpdateRequestEraseFlash:

                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceUpdateRequestBootInfo;
                    informmationViewModelService.ChangeTimerWorkInterval(10);

                break;

                case (int)MwsStatusesEnum.DeviceUpdateResponsData:

                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceUpdateRequestBootInfo;
                    informmationViewModelService.ChangeTimerWorkInterval(10); 
                break;

                case (int)MwsStatusesEnum.DeviceSetDefaultSettings: 
                    informmationViewModelService.ChangeUpdatingProgressBarValue(3500);
                    //Сброс на заводские настройки
                    bufferTxData = new byte[4];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 1;
                    bufferTxData[2] = 0x70;
                    bufferTxData[3] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 4);

                    informmationViewModelService.ChangeTimerWorkInterval(250);
                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceResetAfterSettingDefaultSettings;
                    break;
                    
                case (int)MwsStatusesEnum.DeviceResetAfterSettingDefaultSettings:  
                    bufferTxData = new byte[4];
                    bufferTxData[0] = 0x44;
                    bufferTxData[1] = 1;
                    bufferTxData[2] = 0xFF;
                    bufferTxData[3] = informmationViewModelService.CalcCRC(bufferTxData, bufferTxData[1] + 2);

                    informmationViewModelService.WriteData(bufferTxData, 4);

                    mWSEntity.CommandStatus = 0;
                    informmationViewModelService.ChangeTimerWorkInterval(50);
                      
                    informmationViewModelService.ChangeUpdatingProgressBarValue(6144);
                    informmationViewModelService.UpdateWindowProgresBarStatus("Заводские настройки установлены");
                    break;

                default:

                break;
            }
            
        }



        public void ReadBytesFromComPortCallback(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {

                int numRxByte = (sender as SerialPort).Read(mWSEntity.MwsConfigurationVariables.bufferRxData, 0, 64);

                Array.Copy(mWSEntity.MwsConfigurationVariables.bufferRxData, 0, mWSEntity.MwsConfigurationVariables.bufferRxDataInt, mWSEntity.MwsConfigurationVariables.indexRxDataInt, numRxByte);

                if ((mWSEntity.MwsConfigurationVariables.indexRxDataBoot + numRxByte) > 64)
                    mWSEntity.MwsConfigurationVariables.indexRxDataBoot = 0;

                Array.Copy(mWSEntity.MwsConfigurationVariables.bufferRxData, 0, mWSEntity.MwsConfigurationVariables.bufferRxDataBoot, mWSEntity.MwsConfigurationVariables.indexRxDataBoot, numRxByte);

                mWSEntity.MwsConfigurationVariables.indexRxDataInt += numRxByte;
                mWSEntity.MwsConfigurationVariables.indexRxDataBoot += numRxByte;


                do
                {
                    if (mWSEntity.MwsConfigurationVariables.indexRxDataInt < 4)
                        break;

                    if (((mWSEntity.MwsConfigurationVariables.bufferRxDataInt[0] != 0x45) && (mWSEntity.MwsConfigurationVariables.bufferRxDataInt[0] != 0x44))
                        || (mWSEntity.MwsConfigurationVariables.bufferRxDataInt[1] > 125))
                    {
                        informmationViewModelService.DeleteRxBytes(ref mWSEntity.MwsConfigurationVariables.bufferRxDataInt, 1, ref mWSEntity.MwsConfigurationVariables.indexRxDataInt);

                        continue;
                    }

                    if (mWSEntity.MwsConfigurationVariables.indexRxDataInt < (mWSEntity.MwsConfigurationVariables.bufferRxDataInt[1] + 3))
                        break;

                    if (mWSEntity.MwsConfigurationVariables.bufferRxDataInt[mWSEntity.MwsConfigurationVariables.bufferRxDataInt[1] + 2] != informmationViewModelService.CalcCRC(mWSEntity.MwsConfigurationVariables.bufferRxDataInt, mWSEntity.MwsConfigurationVariables.bufferRxDataInt[1] + 2))
                    {
                        informmationViewModelService.DeleteRxBytes(ref mWSEntity.MwsConfigurationVariables.bufferRxDataInt, 1, ref mWSEntity.MwsConfigurationVariables.indexRxDataInt);
                        continue;
                    }

                    if (mWSEntity.MwsConfigurationVariables.bufferRxDataInt[0] == 0x45)
                    {
                        DecodeIntResponse(mWSEntity.MwsConfigurationVariables.bufferRxDataInt);
                    }

                    informmationViewModelService.DeleteRxBytes(ref mWSEntity.MwsConfigurationVariables.bufferRxDataInt, mWSEntity.MwsConfigurationVariables.bufferRxDataInt[1] + 3, ref mWSEntity.MwsConfigurationVariables.indexRxDataInt);
                }
                while (true);

                if ((mWSEntity.MwsConfigurationVariables.indexRxDataBoot >= mWSEntity.MwsConfigurationVariables.updateCountBytesRX) && (mWSEntity.MwsConfigurationVariables.updateCountBytesRX != 0))
                {
                    DecodeBootloader(mWSEntity.MwsConfigurationVariables.bufferRxDataInt);
                }
                
            }
            catch (Exception ex)
            {
                return;
            }

        }


        public void DecodeIntResponse(byte[] bufferRxDataInt)
        { 

            //разбор сообщений сообщений от MWS
            if (bufferRxDataInt[2] == 0x71)
            {
                 
                ///Mashaling realTimeData
                byte[] realTimeData = new byte[Marshal.SizeOf<MwsRealTimeData>()];
                Array.Copy(bufferRxDataInt, 3, realTimeData, 0, Marshal.SizeOf<MwsRealTimeData>());
                MwsRealTimeData realTimeDataStruct = informmationViewModelService.ReadStruct<MwsRealTimeData>(realTimeData);
                MwsRealTimeDataClass mwsRealTimeDataClass = informmationViewModelService.LoadRealTimeDataFromStruct(realTimeDataStruct);
                mWSEntity.MwsRealTimeData = mwsRealTimeDataClass.Copy();
                mWSEntity.MwsConfigurationVariables.CountNotUnansweredCommands71 = 0;
                mWSEntity.DistanceArrayForEstabilishing = informmationViewModelService.AddDistanceToArray(mWSEntity.DistanceArrayForEstabilishing, mwsRealTimeDataClass.Distance);
                mWSEntity.IsStable = informmationViewModelService.CalculateIsDistanceValueStable(mWSEntity.DistanceArrayForEstabilishing);
                 
                    //updateInformation FormData
                    RaisePropertyChanged(nameof(HeightOfFuelInTheTank)); 
                    //updateInformation FormData   

                ///Mashaling realTimeData   

            }
            else if (bufferRxDataInt[2] == 0x80)
            {
                ///Mashaling commonData
                byte[] commonData = new byte[Marshal.SizeOf<MwsCommonData>()];
                Array.Copy(bufferRxDataInt, 3, commonData, 0, commonData.Length);
                MwsCommonData commonDataStruct = informmationViewModelService.ReadStruct<MwsCommonData>(commonData);
                MwsCommonDataClass mwsCommonDataClass = informmationViewModelService.LoadCommonDataFromStruct(commonDataStruct);
                mWSEntity.MwsCommonData = mwsCommonDataClass.Copy();
                ///Mashaling commonData
                
                //Установка типа датчика и апаратной версии 
                mWSEntity.MwsConfigurationVariables.updateTypeDevice = 0x35;
                mWSEntity.MwsConfigurationVariables.updateHardDevice = commonDataStruct.hardVersion;

                mWSEntity.CommandStatus = (int)MwsStatusesEnum.Command80Accepted;
            }
            else if (bufferRxDataInt[2] == 0x85)
            {
                int pass = bufferRxDataInt[8] | (bufferRxDataInt[9] << 8); 

  
                if (pass == mWSEntity.MwsConfigurationVariables.Pass)
                {
#if PROD
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x000;
#else
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800;
#endif
                    mWSEntity.MwsConfigurationVariables.ConfirmAddress = -1;
                    mWSEntity.MwsConfigurationVariables.CountFF = 0;

                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.Command85Accepted; 
                }

            }
            else if (bufferRxDataInt[2] == 0x90)
            {
                mWSEntity.MwsConfigurationVariables.ConfirmAddress = bufferRxDataInt[3] | (bufferRxDataInt[4] << 8);
                mWSEntity.MwsConfigurationVariables.CountFF = 0;
                 
               

                for (int i = 0; i < bufferRxDataInt[5]; i++)
                {
                   mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead[mWSEntity.MwsConfigurationVariables.ConfirmAddress + i] = bufferRxDataInt[6 + i];

                    if (bufferRxDataInt[6 + i] == 0xFF)
                        mWSEntity.MwsConfigurationVariables.CountFF++;
                }   

                if (mWSEntity.MwsConfigurationVariables.CurrentAddress == mWSEntity.MwsConfigurationVariables.ConfirmAddress) {

                    mWSEntity.MwsConfigurationVariables.CurrentAddress += 32;
                    informmationViewModelService.ChangeProgressBarValue(mWSEntity.MwsConfigurationVariables.CurrentAddress);// progress bar
                }

                if (mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x020)
                {
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800; 
                }

                else if (mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x840)
                {
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x1000; 
                }
                else if ((mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x1800) ||
                    (mWSEntity.MwsConfigurationVariables.CountFF == 32 && mWSEntity.MwsConfigurationVariables.CurrentAddress > 0x1000))
                {

#if PROD
                     ///Mashaling prod settings
                    byte[] prodSettingsData = new byte[Marshal.SizeOf<MwsProdSettings>()];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead, 0x000, prodSettingsData, 0, prodSettingsData.Length);
                    MwsProdSettings prodSettingsStruct = informmationViewModelService.ReadStruct<MwsProdSettings>(prodSettingsData);
                    MwsProdSettingsClass mwsProdSettingsData = informmationViewModelService.LoadProdSettingsFromStruct(prodSettingsStruct);
                    mWSEntity.MwsProdSettingsClass = mwsProdSettingsData.Copy();
                    ///Mashaling urser settings  
#endif


                    ///Mashaling user settings
                    byte[] usersSettingsData = new byte[Marshal.SizeOf<MwsUserSettings>()];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead, 0x800, usersSettingsData, 0, usersSettingsData.Length);
                    MwsUserSettings userSettingsStruct = informmationViewModelService.ReadStruct<MwsUserSettings>(usersSettingsData);
                    MwsUserSettingsClass mwsUsesrSettingsData = informmationViewModelService.LoadUsersSettingsFromStruct(userSettingsStruct);
                    mWSEntity.MwsUserSettings = mwsUsesrSettingsData.Copy(); 
                    ///Mashaling urser settings


                    ///Mashaling calibr table
                    byte[] calibrData = new byte[Marshal.SizeOf<MwsTable>()];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.bufferFlashDataForRead, 0x1000, calibrData, 0, calibrData.Length);
                    MwsTable mwsTableStruct = informmationViewModelService.ReadStruct<MwsTable>(calibrData);
                    MwsTableClass mwsTableData = informmationViewModelService.LoadCalibrTableFromStruct(mwsTableStruct);
                    mWSEntity.MwsTable = mwsTableData.Copy();
                    ///Mashaling
                     

                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.Command90Accepted;

                    //Insert information tank FormData
                    if (mWSEntity.MwsTable.Rows?.LastOrDefault() != null && mWSEntity.MwsTable.Rows?.FirstOrDefault() != null)
                    {
                        TopVolumeTank = mWSEntity.MwsTable.Rows.LastOrDefault().volume.ToString() + " l";
                        TopDistanceTank = mWSEntity.MwsTable.Rows.LastOrDefault().distance.ToString() + " mm";
                        BottomVolumeTank = mWSEntity.MwsTable.Rows.FirstOrDefault().volume.ToString() + " l";
                        BottomDistanceTank = mWSEntity.MwsTable.Rows.FirstOrDefault().distance.ToString() + " mm";
                    }

                    informmationViewModelService.ChangeProgressBarValue(6144);// progress bar
                    //Insert information tank FormData
                     

                    // делаю не мертвый режим потому что датчик точно подключен
                    mWSEntity.MwsConfigurationVariables.deadMode = false;   
                }

            }
            else if (bufferRxDataInt[2] == 0x91)
            {

                mWSEntity.MwsConfigurationVariables.ConfirmAddress = bufferRxDataInt[3] | (bufferRxDataInt[4] << 8); 
                 
                if (mWSEntity.MwsConfigurationVariables.CurrentAddress == mWSEntity.MwsConfigurationVariables.ConfirmAddress)
                {
                    mWSEntity.MwsConfigurationVariables.CurrentAddress += 0x800; 
                }

                if (mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x1800)
                {
#if PROD
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x000;
#else
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800;
#endif

                    mWSEntity.MwsConfigurationVariables.ConfirmAddress = 10000;


                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceFlashWrite;
                }  
            }
            else if (bufferRxDataInt[2] == 0x92)
            {

                mWSEntity.MwsConfigurationVariables.ConfirmAddress = bufferRxDataInt[3] | (bufferRxDataInt[4] << 8); 

                 
                // mWSEntity.MwsConfigurationVariables
                if (mWSEntity.MwsConfigurationVariables.CurrentAddress == mWSEntity.MwsConfigurationVariables.ConfirmAddress)
                {
                    mWSEntity.MwsConfigurationVariables.CurrentAddress += 32; 
                }


                if (mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x020) {
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x800; 
                }
                else if (mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x840) {
                    mWSEntity.MwsConfigurationVariables.CurrentAddress = 0x1000; 
                }
                else if ((mWSEntity.MwsConfigurationVariables.CurrentAddress == 0x1800) || (mWSEntity.MwsConfigurationVariables.CountFF == 32 && mWSEntity.MwsConfigurationVariables.CurrentAddress > 0x1000))
                {
                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceReset;
                    informmationViewModelService.ChangeProgressBarValue(4000);// progress bar
                } 
            } 


        }



        public void DecodeBootloader(byte[] bytes)
        {
            byte[] bufferTxData;

            switch (mWSEntity.CommandStatus)
            {
                case (int)MwsStatusesEnum.DeviceUpdateRequestBootInfo:

                    if (mWSEntity.MwsConfigurationVariables.bufferRxDataBoot[mWSEntity.MwsConfigurationVariables.indexRxDataBoot - 1] != 0xA6)
                        return;

                    if ((mWSEntity.MwsConfigurationVariables.bufferRxDataBoot[mWSEntity.MwsConfigurationVariables.indexRxDataBoot - 4] != mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length - 4]) ||
                        (mWSEntity.MwsConfigurationVariables.bufferRxDataBoot[mWSEntity.MwsConfigurationVariables.indexRxDataBoot - 3] != mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length - 1]))
                    {
                        MessageBox.Show(localizedStrings["strIsNotIntended"], localizedStrings["strError"]);
                        mWSEntity.CommandStatus = 0;
                    }
                    else
                    {

                        mWSEntity.MwsConfigurationVariables.indexRxDataBoot = 0;
                        mWSEntity.MwsConfigurationVariables.updateCountBytesRX = 2;
                         
                        if (mWSEntity.MwsConfigurationVariables.updateTypeDevice == 0x35)
                        {
                            // MWS
                            bufferTxData = new byte[4];
                            bufferTxData[0] = 0x67;
                            bufferTxData[1] = 0x5A;
                            bufferTxData[2] = 0xF2;
                            bufferTxData[3] = 0x36;

                            informmationViewModelService.DiscardInBuffer();
                            informmationViewModelService.DiscardOutBuffer();
                            informmationViewModelService.WriteData(bufferTxData, 4);
                        }

                        mWSEntity.CommandStatus = (int)MwsStatusesEnum.DviceUpdateRequestEraseFlash;

                        informmationViewModelService.ChangeTimerWorkInterval(5000);
                    }

                    break;

                case (int)MwsStatusesEnum.DviceUpdateRequestEraseFlash:

                    if (mWSEntity.MwsConfigurationVariables.bufferRxDataBoot[mWSEntity.MwsConfigurationVariables.indexRxDataBoot - 2] != 0x63 ||
                        mWSEntity.MwsConfigurationVariables.bufferRxDataBoot[mWSEntity.MwsConfigurationVariables.indexRxDataBoot - 1] != 0x3A)
                        return;

                    mWSEntity.MwsConfigurationVariables.indexRxDataBoot = 0;
                    mWSEntity.MwsConfigurationVariables.updateCountBytesRX = 1;

                    informmationViewModelService.DiscardInBuffer();
                    informmationViewModelService.DiscardOutBuffer();

                    bufferTxData = new byte[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] + 5];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.updateBufferFileCOD, mWSEntity.MwsConfigurationVariables.updateIndexTRX, bufferTxData, 0, mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] + 5);
                    informmationViewModelService.WriteData(bufferTxData, bufferTxData.Length);

                    mWSEntity.MwsConfigurationVariables.updateIndexTRX += mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] + 5;

                    mWSEntity.CommandStatus = (int)MwsStatusesEnum.DeviceUpdateResponsData;


                    informmationViewModelService.ChangeTimerWorkInterval(500);

                    break;

                case (int)MwsStatusesEnum.DeviceUpdateResponsData:

                    if (mWSEntity.MwsConfigurationVariables.bufferRxDataBoot[mWSEntity.MwsConfigurationVariables.indexRxDataBoot - 1] != 0x3A)
                        return;

                    mWSEntity.MwsConfigurationVariables.indexRxDataBoot = 0;
                    mWSEntity.MwsConfigurationVariables.updateCountBytesRX = 1;

                    bufferTxData = new byte[mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] + 5];
                    Array.Copy(mWSEntity.MwsConfigurationVariables.updateBufferFileCOD, mWSEntity.MwsConfigurationVariables.updateIndexTRX, bufferTxData, 0, mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] + 5);
                    informmationViewModelService.WriteData(bufferTxData, bufferTxData.Length);

                    
                    informmationViewModelService.ChangeUpdatingProgressBarValue(mWSEntity.MwsConfigurationVariables.updateIndexTRX * 6144 / mWSEntity.MwsConfigurationVariables.updateBufferFileCOD.Length); 

                    if (mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] == 0)
                    {
                        informmationViewModelService.ChangeUpdatingProgressBarValue(0);

                        mWSEntity.CommandStatus = 0; 

                        informmationViewModelService.ChangeTimerWorkInterval(50);

                        MessageBox.Show(localizedStrings["strUpdateSuccessful"], localizedStrings["strSuccess"], MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly); 
                        
                        informmationViewModelService.UpdateWindowProgresBarStatus(localizedStrings["Updated"]);
                    }
                    else
                    {
                        mWSEntity.MwsConfigurationVariables.updateIndexTRX += mWSEntity.MwsConfigurationVariables.updateBufferFileCOD[mWSEntity.MwsConfigurationVariables.updateIndexTRX] + 5;

                        informmationViewModelService.ChangeTimerWorkInterval(500);
                    }

                break;
            }

        }



    }
}
