

using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Entity.MWSStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MwsRealTimeData
    {
        public float distance;
        public float level;
        public float volume;
        public short temp;
        public ushort n;
        public uint flags;
    }
}
