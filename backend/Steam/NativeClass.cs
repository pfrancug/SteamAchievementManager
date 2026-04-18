using System.Runtime.InteropServices;

namespace SAM.Backend.Steam;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
internal struct NativeClass
{
    public IntPtr VirtualTable;
}
