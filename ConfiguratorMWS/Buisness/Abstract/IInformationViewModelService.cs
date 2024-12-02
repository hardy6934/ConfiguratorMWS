
using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using System.IO.Ports;

namespace ConfiguratorMWS.Buisness.Abstract
{
    public interface IInformationViewModelService
    {
        //Struct->Model | Model->Struct
        public T ReadStruct<T>(byte[] buffer);
        public MwsTableClass LoadCalibrTableFromStruct(MwsTable mwsTableStruct);
        public MwsProdSettingsClass LoadProdSettingsFromStruct(MwsProdSettings settings);
        public MwsUserSettingsClass LoadUsersSettingsFromStruct(MwsUserSettings settings);
        public MwsRealTimeDataClass LoadRealTimeDataFromStruct(MwsRealTimeData structData);
        public MwsCommonDataClass LoadCommonDataFromStruct(MwsCommonData structData);
        public float[] AddDistanceToArray(float[] array, float newValue); 
        public int CalculateIsDistanceValueStable(float[] array);


        // Read byte from callback
        public void DeleteRxBytes(ref byte[] buff, int num, ref int index);
        public byte CalcCRC(byte[] data, int len);


        //mwsRepo
        public MWSEntity GetEntity();
        public void ChangeProgressBarValue(int value);
        public void IncrementingProgressBarValue(int value);
        public void ChangeUpdatingProgressBarValue(int value);
        public void UpdateWindowProgresBarStatus(string val);
        public void GeneralWindowProgressBarStatus(string val);



        //SerialPortRepo
        public List<string> GetAvailableComPortNames();
        public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback);
        bool IsPortAvailable(string portName);
        bool CloseConnectionWithComPort();
        public void WriteData(byte[] data, int count);
        public void DiscardInBuffer();
        public void DiscardOutBuffer();


        //TimerRepo
        public void TimerWork(EventHandler timerCallback);
        public void ChangeTimerWorkInterval(int interval);

    }
}
