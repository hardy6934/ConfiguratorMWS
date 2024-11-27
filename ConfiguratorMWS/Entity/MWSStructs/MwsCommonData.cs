

using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Entity.MWSStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct MwsCommonData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] serialNumberProd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] serialNumber;
        public byte sensorType;
        public byte hardVersion;
        public byte vProtocola;
        public byte softVersion;

        
    }
}
