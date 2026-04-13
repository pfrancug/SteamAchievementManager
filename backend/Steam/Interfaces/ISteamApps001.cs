using System.Runtime.InteropServices;

namespace SAM.Backend.Steam.Interfaces;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ISteamApps001
{
    public IntPtr GetAppData;
}
