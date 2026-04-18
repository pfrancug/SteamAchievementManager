using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamUser012 : NativeWrapper<ISteamUser012>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeLoggedOn(IntPtr self);

    public bool IsLoggedIn()
    {
        var call = GetFunction<NativeLoggedOn>(Functions.LoggedOn);
        return call(ObjectAddress);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void NativeGetSteamId(IntPtr self, out ulong steamId);

    public ulong GetSteamId()
    {
        var call = GetFunction<NativeGetSteamId>(Functions.GetSteamID);
        call(ObjectAddress, out ulong steamId);
        return steamId;
    }
}
