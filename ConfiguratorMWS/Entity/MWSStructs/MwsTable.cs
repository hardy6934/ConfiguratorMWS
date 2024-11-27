

using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Entity.MWSStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MwsTable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public MwsRow[] rows;
    }
}
