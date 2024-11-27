using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Entity.MWSStructs
{ 
    // MWS User Settings
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MwsUserSettings
    {
        public ushort flashUserSetYear;
        public byte flashUserSetMonth;
        public byte flashUserSetDay;
        public uint flashUserSerialNumber;
        public ushort flashUserpassword;

        public uint flashUserConfigFlags;
        public uint flashUserBaudRateRs;
        public uint flashUserSpeedCan;

        public byte flashUserAdrSensor;
        public ushort flashUserLlsMinN;
        public ushort flashUserLlsMaxN;

        public byte flashUserSaCan;

        public float flashUserTankHeight;
        public ushort flashUserAverageS;
        public float flashUserThreshold;
    }
}
