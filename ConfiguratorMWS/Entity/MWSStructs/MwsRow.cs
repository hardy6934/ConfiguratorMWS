
using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Entity.MWSStructs
{

    // MWS Row
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MwsRow
    {
        public float distance;
        public float volume;
    }
}
