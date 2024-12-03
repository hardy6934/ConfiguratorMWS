using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Runtime.InteropServices;


namespace ConfiguratorMWS.Buisness.Service
{
    public class InformationViewModelService : IInformationViewModelService
    {
        private readonly IMWSRepository mWSRepository;
        private readonly ITimerRepository timerRepository;
        private readonly ISerialPortRepository serialPortRepository;

        public InformationViewModelService(IMWSRepository mWSRepository, ITimerRepository timerRepository, ISerialPortRepository serialPortRepository) {
            this.mWSRepository = mWSRepository;
            this.timerRepository = timerRepository;
            this.serialPortRepository = serialPortRepository;
        }

        //Struct->Model | Model->Struct
        public T ReadStruct<T>(byte[] buffer) {
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return temp;
        }

        public MwsTableClass LoadCalibrTableFromStruct(MwsTable mwsTableStruct)
        {

            var MwsTable = new MwsTableClass() { Rows = new ObservableCollection<MwsRowClass>() };
            int k = 0;
            foreach (var row in mwsTableStruct.rows)
            {
                k++;
                if (row.distance == 100000.0f)
                    break;

                MwsTable.Rows.Add(new MwsRowClass
                {
                    Number = k,
                    Distance = (float)Math.Round(row.distance * 1000),
                    Volume = row.volume
                });
            }
            return MwsTable;
        }

        public MwsProdSettingsClass LoadProdSettingsFromStruct(MwsProdSettings settings)
        {
            return new MwsProdSettingsClass
            {
                ProdYear = settings.prodYear,
                ProdMonth = settings.prodMonth,
                ProdDay = settings.prodDay,
                SerialNumber = settings.serialNumber,
                DeviceType = settings.deviceType,
                Password = settings.password
            }; 
        }


        public MwsUserSettingsClass LoadUsersSettingsFromStruct(MwsUserSettings settings) {
            return new MwsUserSettingsClass
            {
                FlashUserSetYear = settings.flashUserSetYear,
                FlashUserSetMonth = settings.flashUserSetMonth,
                FlashUserSetDay = settings.flashUserSetDay,
                FlashUserSerialNumber = settings.flashUserSerialNumber,
                FlashUserpassword = settings.flashUserpassword,
                FlashUserConfigFlags = settings.flashUserConfigFlags,
                FlashUserBaudRateRs = settings.flashUserBaudRateRs,
                FlashUserSpeedCan = settings.flashUserSpeedCan,
                FlashUserAdrSensor = settings.flashUserAdrSensor,
                FlashUserLlsMinN = settings.flashUserLlsMinN,
                FlashUserLlsMaxN = settings.flashUserLlsMaxN,
                FlashUserSaCan = settings.flashUserSaCan,
                FlashUserTankHeight = settings.flashUserTankHeight,
                FlashUserAverageS = settings.flashUserAverageS,
                FlashUserThreshold = settings.flashUserThreshold
            };
        } 

        public MwsRealTimeDataClass LoadRealTimeDataFromStruct(MwsRealTimeData structData) {
            return new MwsRealTimeDataClass
            {
                Distance = (float)Math.Round(structData.distance * 1000),
                Level = structData.level,
                Volume = structData.volume,
                Temp = structData.temp,
                N = structData.n,
                Flags = structData.flags
            };
        }

        public MwsCommonDataClass LoadCommonDataFromStruct(MwsCommonData structData) {
            byte[] serNumber = new byte[4];
            Array.Copy(structData.serialNumber, serNumber, structData.serialNumber.Length);
            serNumber[3] = 0x00;

            byte[] serNumberProd = new byte[4];
            Array.Copy(structData.serialNumberProd, serNumberProd, structData.serialNumberProd.Length);
            serNumberProd[3] = 0x00;


            return new MwsCommonDataClass
            {
                SensorType = structData.sensorType,
                SerialNumber = BitConverter.ToUInt32(serNumberProd),
                HardVersion = ((structData.hardVersion + 10) / 10.0).ToString("F1").Replace(',', '.'),
                SoftVersion = ((structData.softVersion + 10) / 10.0).ToString("F1").Replace(',', '.')
            };
        }

        public float[] AddDistanceToArray(float[] array, float newValue) { 

            float[] newArray = new float[array.Length]; 

            newArray[0] = newValue;
             
            for (int i = 0; i < array.Length - 1; i++)
            {
                newArray[i + 1] = array[i];
            }
            return newArray;
        } 
        public int CalculateIsDistanceValueStable(float[] array) {

            /// 0 - not stable /// 1 - stable /// 2 - (шум/препятствие на пуити) /// 3 - (препятствие в мертвой зоне)

            if (array.Contains(0) || array.Length == 0) return 0;

            var flag = mWSRepository.GetMwsFlag();

            if ((flag & (1 << 8)) != 0)
            {
                // Первый кейс
                float averageValue = 0; // Среднее значение в массиве
                float threshold = 20.0f; // Порог отклонения

                foreach (float value in array)
                {
                    averageValue += value;
                }
                averageValue = averageValue / array.Length;

                foreach (float value in array)
                {
                    if (Math.Abs(value - averageValue) > threshold)
                    {
                        return 0;
                    }
                }
                return 1;
            }
            else if ((flag & (1 << 9)) != 0)
            {
                // (шум/препятствие на пуити) 
                return 2;
            } 
            else if ((flag & (1 << 10)) != 0)
            {
                // (препятствие в мертвой зоне)
                return 3;
            }
            else
            {
                
                return 0;
            }
             

        } 



        // Read byte from callback
        public void DeleteRxBytes(ref byte[] buff, int num, ref int index) {
            for (int i = num; i < index; i++)
            {
                buff[i - num] = buff[i];
            }

            index -= num;
        }

        public byte CalcCRC(byte[] data, int len) {
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


        //mwsRepo
        public MWSEntity GetEntity() {
            return mWSRepository.GetEntity();
        }
        public void ChangeProgressBarValue(int value) { 
            mWSRepository.ChangeProgressBarValue(value);
        }

        public void ChangeUpdatingProgressBarValue(int value)
        {
            mWSRepository.ChangeUpdatingProgressBarValue(value);
        }
        public void IncrementingProgressBarValue(int value)
        {
            mWSRepository.IncremeentingProgressBarValue(value);
        }
        public void UpdateWindowProgresBarStatus(string val) 
        {
            mWSRepository.UpdateWindowProgresBarStatus(val);
        }
        public void GeneralWindowProgressBarStatus(string val)
        {
            mWSRepository.GeneralWindowProgressBarStatus(val);
        }


        //SerialPortRepo
        public List<string> GetAvailableComPortNames() { 
            return serialPortRepository.GetAvailableComPortNames();
        }
        public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback) { 
            return serialPortRepository.ConnectWithComPort(portName, boundRate, callback);
        }
        public bool IsPortAvailable(string portName) {
            return serialPortRepository.IsPortAvailable(portName);
        }
        public bool CloseConnectionWithComPort() { 
            return serialPortRepository.CloseConnectionWithComPort();
        }
        public void WriteData(byte[] data, int count) {
            serialPortRepository.WriteData(data, count);
        }
        public void DiscardInBuffer() { 
            serialPortRepository.DiscardInBuffer();
        }
        public void DiscardOutBuffer() {
            serialPortRepository.DiscardOutBuffer();
        }




        //TimerRepo
        public void TimerWork(EventHandler timerCallback) { 
            timerRepository.TimerWork(timerCallback);
        }
        public void ChangeTimerWorkInterval(int interval) {
            timerRepository.ChangeTimerWorkInterval(interval);

        }

    }
}
