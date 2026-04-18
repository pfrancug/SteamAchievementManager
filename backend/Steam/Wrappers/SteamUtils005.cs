using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamUtils005 : NativeWrapper<ISteamUtils005>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate uint NativeGetAppId(IntPtr self);

    public uint GetAppId()
    {
        var call = GetFunction<NativeGetAppId>(Functions.GetAppID);
        return call(ObjectAddress);
    }
}
