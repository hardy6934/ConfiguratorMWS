using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using System.IO.Ports; 
using System.Windows.Threading;

namespace ConfiguratorMWS.Data.Repository
{
    public class MWSRepository : IMWSRepository
    {
        private DispatcherTimer timer;
        private MWSEntity mWSEntity;

        public MWSRepository(MWSEntity mWSEntity) { 
            this.mWSEntity = mWSEntity;
            timer = new DispatcherTimer();
        }

        private SerialPort currentPort;


        public List<string> GetAvailableComPortNames()
        { 
            var qwe = SerialPort.GetPortNames().ToList();
            return qwe; 
        }

        public bool ConnectWithComPort(string portName, int boundRate, SerialDataReceivedEventHandler callback)
        {  
             
            if (currentPort == null || !currentPort.IsOpen)
            {

                try
                {
                    currentPort = new SerialPort();
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


        public void SetIsConnectedTrue()
        {
            mWSEntity.isConnected = true; 
        }

        public void SetIsConnectedFalse()
        {
            mWSEntity.isConnected = false;
        }

        public void TimerWork(EventHandler timerCallback)
        {
            //таймер
            //установка начальных значений работы таймера
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += timerCallback;
            timer.Start();
        }
        public void ChangeTimerWorkInterval(int interval)
        { 
            timer.Stop();
            timer.Interval = TimeSpan.FromMilliseconds(3100);
            timer.Start();
        }

        public void WriteData(byte[] data, int count) {
            if (currentPort != null && currentPort.IsOpen)
            {
                currentPort.Write(data, 0, count);
            }
            
        }

        public byte[] DecodeIntResponse(byte[] bufferRxDataInt)
        {
            
            //all readed data buffer
            byte[] bufferFlashData = new byte[6144];

            //разбор сообщений сообщений от MWS
            if (bufferRxDataInt[2] == 0x71)
            {
                
                if (mWSEntity.sensorType == 0xAA)
                {
                    // MWS

                    mWSEntity.Distance = BitConverter.ToSingle(bufferRxDataInt, 3);
                    mWSEntity.Level = BitConverter.ToSingle(bufferRxDataInt, 7);
                    mWSEntity.Volume = BitConverter.ToSingle(bufferRxDataInt, 11);
                    mWSEntity.Temp = BitConverter.ToInt16(bufferRxDataInt, 15);
                    mWSEntity.nOmni = BitConverter.ToUInt16(bufferRxDataInt, 17);
                    mWSEntity.Flags = BitConverter.ToUInt32(bufferRxDataInt, 19);

                }
                 

            }
            else if (bufferRxDataInt[2] == 0x80)
            {
                byte[] serNumber = new byte[4];
                Array.Copy(bufferRxDataInt, 6, serNumber, 0, 4);
                serNumber[3] = 0x00;

                mWSEntity.SerialNumber = BitConverter.ToUInt32(serNumber, 0);
                mWSEntity.SensorType = bufferRxDataInt[9];
                mWSEntity.HardVersion = bufferRxDataInt[10].ToString();
                mWSEntity.SoftVersion = bufferRxDataInt[12].ToString();

                  
                mWSEntity.CommandStatus = (int)MWSEntity.MwsStatuses.Command80Accepted;
            }
            else if (bufferRxDataInt[2] == 0x85)
            {
                int pass = bufferRxDataInt[8] | (bufferRxDataInt[9] << 8);
                int confirmPass = 0;

#if PROD
                        if (devicesOur[indexCurrentDevice].typeDevice == 0xAA)
                            confirmPass = 0x1223;                
#else
                if (mWSEntity.sensorType == 0xAA)
                    confirmPass = 0xAC09;
#endif

                if (pass == confirmPass) {
                    
                    mWSEntity.CommandStatus = (int)MWSEntity.MwsStatuses.Command85Accepted;
                }
                    
            }
            else if (bufferRxDataInt[2] == 0x90)
            {
                mWSEntity.CommandStatus = (int)MWSEntity.MwsStatuses.Command90Accepted;
                mWSEntity.CommandLastReadedBytes = bufferRxDataInt[3] | (bufferRxDataInt[4] << 8);
                mWSEntity.CountFF = 0;

                for (int i = 0; i < bufferRxDataInt[5]; i++)
                {
                    bufferFlashData[mWSEntity.CommandLastReadedBytes + i] = bufferRxDataInt[6 + i];

                    if (bufferRxDataInt[6 + i] == 0xFF)
                        mWSEntity.CountFF++;
                }
            }
            else if (bufferRxDataInt[2] == 0x91)
            {
                mWSEntity.CommandStatus = (int)MWSEntity.MwsStatuses.Command91Accepted;
                mWSEntity.CommandLastReadedBytes = bufferRxDataInt[3] | (bufferRxDataInt[4] << 8);
            }
            else if (bufferRxDataInt[2] == 0x92)
            {
                mWSEntity.CommandStatus = (int)MWSEntity.MwsStatuses.Command92Accepted;
                mWSEntity.CommandLastReadedBytes = bufferRxDataInt[3] | (bufferRxDataInt[4] << 8);
            }

            return bufferFlashData;

        }

        public void DecodeBootloader(byte[] bytes)
        {

            //switch (State)
            //{
            //    case enumState.deviceUpdateRequestBootInfo:

            //        if (bufferRxDataBoot[indexRxDataBoot - 1] != 0xA6)
            //            return;

            //        if ((bufferRxDataBoot[indexRxDataBoot - 4] != updateBufferFileCOD[updateBufferFileCOD.Length - 4]) ||
            //            (bufferRxDataBoot[indexRxDataBoot - 3] != updateBufferFileCOD[updateBufferFileCOD.Length - 1]))
            //        {
            //            MessageBox.Show(strTheFileIsNotIntended, strError, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //            if (deadMode)
            //            {
            //                State = enumState.comPortConnected;
            //            }
            //            else
            //            {
            //                indexCurrentDevice = lbDevices.SelectedIndex;
            //                State = enumState.deviceNoAccessAll;
            //            }
            //        }
            //        else
            //        {
            //            loopTimer.Stop();

            //            indexRxDataBoot = 0;
            //            updateCountBytesRX = 2;

            //            // запрос стереть память
            //            if (checkBox19.Checked)
            //                CmdRequestBootloaderErase(updateTypeDevice, 0x37);
            //            else
            //                CmdRequestBootloaderErase(updateTypeDevice, 0x36);

            //            State = enumState.deviceUpdateRequestEraseFlash;

            //            loopTimer.Interval = 5000;
            //            loopTimer.Start();
            //        }

            //        break;

            //    case enumState.deviceUpdateRequestEraseFlash:

            //        if (bufferRxDataBoot[indexRxDataBoot - 2] != 0x63 ||
            //            bufferRxDataBoot[indexRxDataBoot - 1] != 0x3A)
            //            return;

            //        loopTimer.Stop();

            //        indexRxDataBoot = 0;
            //        updateCountBytesRX = 1;

            //        CmdResponsBootloaderData(updateBufferFileCOD, updateIndexTRX);
            //        updateIndexTRX += updateBufferFileCOD[updateIndexTRX] + 5;

            //        toolStripProgressBar1.Value = 0;
            //        toolStripProgressBar1.Maximum = updateBufferFileCOD.Length;

            //        State = enumState.deviceUpdateResponsData;

            //        loopTimer.Interval = 500;
            //        loopTimer.Start();

            //        break;

            //    case enumState.deviceUpdateResponsData:

            //        if (bufferRxDataBoot[indexRxDataBoot - 1] != 0x3A)
            //            return;

            //        loopTimer.Stop();

            //        indexRxDataBoot = 0;
            //        updateCountBytesRX = 1;

            //        CmdResponsBootloaderData(updateBufferFileCOD, updateIndexTRX);

            //        toolStripProgressBar1.Value = updateIndexTRX;

            //        if (updateBufferFileCOD[updateIndexTRX] == 0)
            //        {
            //            toolStripProgressBar1.Value = 0;

            //            MessageBox.Show(strUpdateSuccessful, strSuccess, MessageBoxButtons.OK, MessageBoxIcon.Information);

            //            if (deadMode)
            //            {
            //                State = enumState.comPortConnected;
            //            }
            //            else
            //            {
            //                indexCurrentDevice = lbDevices.SelectedIndex;
            //                State = enumState.deviceNoAccessAll;
            //            }

            //            loopTimer.Interval = 50;
            //            loopTimer.Start();
            //        }
            //        else
            //        {
            //            updateIndexTRX += updateBufferFileCOD[updateIndexTRX] + 5;

            //            loopTimer.Interval = 500;
            //            loopTimer.Start();
            //        }

            //        break;
            //} 

        }
    }
}
