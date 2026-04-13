using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamUtils005 : NativeWrapper<ISteamUtils005>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate uint NativeGetAppId(IntPtr self);

    public uint GetAppId()
    {
        return Call<uint, NativeGetAppId>(Functions.GetAppID, ObjectAddress);
    }
}
