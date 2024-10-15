
using System.IO.Ports;

namespace ConfiguratorMWS.Data.Abstract
{
    public interface IMWSRepository
    {
        public List<string> GetAvailableComPortNames();
        public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback);
        public byte[] DecodeIntResponse(byte[] bytes); 
        public void DecodeBootloader(byte[] bytes); 

        public void SetIsConnectedTrue();
        public void SetIsConnectedFalse();

        public void TimerWork(EventHandler timerCallback);
        public void ChangeTimerWorkInterval(int interval);
        public void WriteData(byte[] data, int count);



    }
}
