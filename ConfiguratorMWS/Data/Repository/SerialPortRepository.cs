
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using System.IO;
using System.IO.Ports;

namespace ConfiguratorMWS.Data.Repository
{
    public class SerialPortRepository : ISerialPortRepository
    {

        private SerialPort currentPort;
        public SerialPortRepository() {
        }


        public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback)
        {
            if (currentPort == null || !currentPort.IsOpen)
            {

                try
                {
                    currentPort = new SerialPort();
                    currentPort.BaudRate = 123457;
                    currentPort.PortName = portName;
                    currentPort.StopBits = StopBits.OnePointFive; 
                    currentPort.Open();

                    Thread.Sleep(500);
                    currentPort.Close();

                    currentPort.ReadTimeout = 1000;
                    currentPort.WriteTimeout = 1000;
                    currentPort.PortName = portName;
                    currentPort.BaudRate = boundRate;
                    currentPort.ReadBufferSize = 64;
                    currentPort.WriteBufferSize = 64;
                    currentPort.DataReceived += callback;
                    currentPort.Open();
                     
                    if (currentPort.IsOpen)
                    {
                        return true;
                    }
                    return false;

                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<string> GetAvailableComPortNames()
        {
            var portNammes = SerialPort.GetPortNames().ToList();
            return portNammes;
        }

        public bool CloseConnectionWithComPort()
        {
            try
            {
                if (currentPort == null || currentPort.IsOpen)
                {
                    currentPort.Close();
                    currentPort.Dispose();
                    if (!currentPort.IsOpen)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
          

        public bool IsPortAvailable(string portName)
        {
            try
            {
                using (var port = new SerialPort(portName))
                {
                    port.Open();
                    port.Close();
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }



        public void WriteData(byte[] data, int count)
        {
            try
            {
                if (currentPort != null && currentPort.IsOpen)
                {
                    currentPort.Write(data, 0, count);
                }
            }
            catch (TimeoutException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public void DiscardInBuffer()
        { 
            currentPort.DiscardInBuffer(); 
        }

        public void DiscardOutBuffer()
        {
            currentPort.DiscardOutBuffer();
        }
    }
}
