using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamApps008 : NativeWrapper<ISteamApps008>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeIsSubscribedApp(IntPtr self, uint gameId);

    public bool IsSubscribedApp(uint gameId)
    {
        var call = GetFunction<NativeIsSubscribedApp>(Functions.IsSubscribedApp);
        return call(ObjectAddress, gameId);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetCurrentGameLanguage(IntPtr self);

    public string? GetCurrentGameLanguage()
    {
        var call = GetFunction<NativeGetCurrentGameLanguage>(Functions.GetCurrentGameLanguage);
        var languagePointer = call(ObjectAddress);
        return NativeStrings.PointerToString(languagePointer);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate uint NativeGetEarliestPurchaseUnixTime(IntPtr self, uint appId);

    public uint GetEarliestPurchaseUnixTime(uint appId)
    {
        var call = GetFunction<NativeGetEarliestPurchaseUnixTime>(
            Functions.GetEarliestPurchaseUnixTime
        );
        return call(ObjectAddress, appId);
    }
}
