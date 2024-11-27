

using System.IO.Ports;

namespace ConfiguratorMWS.Data.Abstract
{
    public interface ISerialPortRepository
    {
        public List<string> GetAvailableComPortNames();
        public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback);
        bool IsPortAvailable(string portName);
        bool CloseConnectionWithComPort(); 
        public void WriteData(byte[] data, int count);

        public void DiscardInBuffer();
        public void DiscardOutBuffer();


    }
}
