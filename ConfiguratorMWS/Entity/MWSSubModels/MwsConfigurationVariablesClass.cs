#define PROD

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsConfigurationVariablesClass
    {
        public int CurrentAddress { get; set; } = 0;
        public int ConfirmAddress { get; set; } = 0;
        public int CountFF { get; set; }
#if PROD
        public int Pass { get; set; } = 0x1223;
#else
        public int Pass { get; set; } = 0xAC09;
#endif

        public int Config { get; set; } = 0x05; 
        public int CountNotUnansweredCommands71 { get; set; } = 0; 

         
        //array for read write settings
        public byte[] bufferFlashDataForWr { get; set; } = new byte[6144];//byte array for write

        //settings for updating firmware 
        public byte[] updateBufferFileCOD;
        public int updateIndexTRX = 0;
        public int updateCountBytesRX = 0;
        public byte updateTypeDevice = 0;
        public byte updateHardDevice = 0;
        public bool deadMode = true;
        //settings for updating firmware 


        // приемные и передающие буферы   
        public byte[] bufferFlashDataForRead = new byte[6144];//byte array for read

        public byte[] bufferRxData = new byte[64];

        public int indexRxDataInt = 0;
        public byte[] bufferRxDataInt = new byte[128];

        public int indexRxDataBoot = 0;
        public byte[] bufferRxDataBoot = new byte[64];

        public byte[] bufferTxData = new byte[64];
        // приемные и передающие буферы



        public MwsConfigurationVariablesClass Clone()
        {
            return new MwsConfigurationVariablesClass
            {
                CurrentAddress = this.CurrentAddress,
                ConfirmAddress = this.ConfirmAddress,
                CountFF = this.CountFF,
                Pass = this.Pass,
                Config = this.Config,
                CountNotUnansweredCommands71 = this.CountNotUnansweredCommands71,
                bufferFlashDataForWr = this.bufferFlashDataForWr,
                updateBufferFileCOD = this.updateBufferFileCOD,
                updateIndexTRX = this.updateIndexTRX,
                updateCountBytesRX = this.updateCountBytesRX,
                updateTypeDevice = this.updateTypeDevice,
                updateHardDevice = this.updateHardDevice,
                deadMode = this.deadMode,

                bufferRxData = this.bufferRxData,
                indexRxDataInt = this.indexRxDataInt,
                bufferRxDataInt = this.bufferRxDataInt,
                indexRxDataBoot = this.indexRxDataBoot,
                bufferRxDataBoot = this.bufferRxDataBoot,
                bufferTxData = this.bufferTxData
            };
        } 

    }
}
