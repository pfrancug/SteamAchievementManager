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
        return Call<bool, NativeIsSubscribedApp>(Functions.IsSubscribedApp, ObjectAddress, gameId);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetCurrentGameLanguage(IntPtr self);

    public string? GetCurrentGameLanguage()
    {
        var languagePointer = Call<IntPtr, NativeGetCurrentGameLanguage>(
            Functions.GetCurrentGameLanguage,
            ObjectAddress
        );
        return NativeStrings.PointerToString(languagePointer);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate uint NativeGetEarliestPurchaseUnixTime(IntPtr self, uint appId);

    public uint GetEarliestPurchaseUnixTime(uint appId)
    {
        return Call<uint, NativeGetEarliestPurchaseUnixTime>(
            Functions.GetEarliestPurchaseUnixTime,
            ObjectAddress,
            appId
        );
    }
}
