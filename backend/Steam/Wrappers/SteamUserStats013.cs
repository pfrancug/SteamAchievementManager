using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamUserStats013 : NativeWrapper<ISteamUserStats013>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeGetStatInt(IntPtr self, IntPtr name, out int data);

    public bool GetStatValue(string name, out int value)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        var call = GetFunction<NativeGetStatInt>(Functions.GetStatInteger);
        return call(ObjectAddress, nativeName.Handle, out value);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeGetStatFloat(IntPtr self, IntPtr name, out float data);

    public bool GetStatValue(string name, out float value)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        var call = GetFunction<NativeGetStatFloat>(Functions.GetStatFloat);
        return call(ObjectAddress, nativeName.Handle, out value);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeSetStatInt(IntPtr self, IntPtr name, int data);

    public bool SetStatValue(string name, int value)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        var call = GetFunction<NativeSetStatInt>(Functions.SetStatInteger);
        return call(ObjectAddress, nativeName.Handle, value);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeSetStatFloat(IntPtr self, IntPtr name, float data);

    public bool SetStatValue(string name, float value)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        var call = GetFunction<NativeSetStatFloat>(Functions.SetStatFloat);
        return call(ObjectAddress, nativeName.Handle, value);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeGetAchievement(
        IntPtr self,
        IntPtr name,
        [MarshalAs(UnmanagedType.I1)] out bool isAchieved
    );

    public bool GetAchievement(string name, out bool isAchieved)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        var call = GetFunction<NativeGetAchievement>(Functions.GetAchievement);
        return call(ObjectAddress, nativeName.Handle, out isAchieved);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeSetAchievement(IntPtr self, IntPtr name);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeClearAchievement(IntPtr self, IntPtr name);

    public bool SetAchievement(string name, bool state)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        if (!state)
        {
            var clear = GetFunction<NativeClearAchievement>(Functions.ClearAchievement);
            return clear(ObjectAddress, nativeName.Handle);
        }
        var set = GetFunction<NativeSetAchievement>(Functions.SetAchievement);
        return set(ObjectAddress, nativeName.Handle);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeGetAchievementAndUnlockTime(
        IntPtr self,
        IntPtr name,
        [MarshalAs(UnmanagedType.I1)] out bool isAchieved,
        out uint unlockTime
    );

    public bool GetAchievementAndUnlockTime(string name, out bool isAchieved, out uint unlockTime)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        var call = GetFunction<NativeGetAchievementAndUnlockTime>(
            Functions.GetAchievementAndUnlockTime
        );
        return call(ObjectAddress, nativeName.Handle, out isAchieved, out unlockTime);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeStoreStats(IntPtr self);

    public bool StoreStats()
    {
        var call = GetFunction<NativeStoreStats>(Functions.StoreStats);
        return call(ObjectAddress);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetAchievementDisplayAttribute(
        IntPtr self,
        IntPtr name,
        IntPtr key
    );

    public string? GetAchievementDisplayAttribute(string name, string key)
    {
        using var nativeName = NativeStrings.StringToStringHandle(name);
        using var nativeKey = NativeStrings.StringToStringHandle(key);
        var call = GetFunction<NativeGetAchievementDisplayAttribute>(
            Functions.GetAchievementDisplayAttribute
        );
        var result = call(ObjectAddress, nativeName.Handle, nativeKey.Handle);
        return NativeStrings.PointerToString(result);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate ulong NativeRequestUserStats(IntPtr self, ulong steamIdUser);

    public ulong RequestUserStats(ulong steamIdUser)
    {
        var call = GetFunction<NativeRequestUserStats>(Functions.RequestUserStats);
        return call(ObjectAddress, steamIdUser);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeResetAllStats(
        IntPtr self,
        [MarshalAs(UnmanagedType.I1)] bool achievementsToo
    );

    public bool ResetAllStats(bool achievementsToo)
    {
        var call = GetFunction<NativeResetAllStats>(Functions.ResetAllStats);
        return call(ObjectAddress, achievementsToo);
    }
}
