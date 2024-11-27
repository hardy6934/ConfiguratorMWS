

using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Entity.MWSStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MwsProdSettings
    { 
        public ushort prodYear;
        public byte prodMonth;
        public byte prodDay;
        public uint serialNumber;
        public byte deviceType;
        public ushort password; 
    }
}
