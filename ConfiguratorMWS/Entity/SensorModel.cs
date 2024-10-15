 

namespace ConfiguratorMWS.Entity
{
    public interface SensorModel
    {
        public uint SensorType { get; set; }
        public uint SerialNumber { get; set; } 
        public string HardVersion { get; set; } 
        public string SoftVersion { get; set; }


        //propertu for reading data
        public int CommandStatus { get; set; }
        public int CommandLastReadedBytes { get; set; }
        public int CountFF { get; set; }
        public int Pass { get; set; } 
        public int Config { get; set; } 

    }
}
