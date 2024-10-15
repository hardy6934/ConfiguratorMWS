
using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using System.IO.Ports;

namespace ConfiguratorMWS.Buisness.Service
{
    public class MWSService : IMWSService
    {
        //private readonly IMWSRepository mwsRepository;
        //public MWSService(IMWSRepository mwsRepository) {
        //    this.mwsRepository = mwsRepository;
        //}

        //public void GetSettings()
        //{
        //     mwsRepository.RequesAlltData(); 
        //}


        //public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback) { 
        //    return mwsRepository.ConnectWithComPort(portName, boundRate, callback);
        //}

        //public List<string> GetAvailableComPortNames() { 
        //    return mwsRepository.GetAvailableComPortNames();
        //}

        //public void SetIsConnectedTrue()
        //{
        //    mwsRepository.SetIsConnectedTrue();
        //}

        //public void SetIsConnectedFalse()
        //{
        //    mwsRepository.SetIsConnectedFalse();
        //}


        //public void ReadBytesFromComPort(object sender, SerialDataReceivedEventArgs e) {
        //    mwsRepository.ReadBytesFromComPort(sender, e);
        //}

    }
}
