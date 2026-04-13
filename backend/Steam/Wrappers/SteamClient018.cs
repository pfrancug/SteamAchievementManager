using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamClient018 : NativeWrapper<ISteamClient018>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int NativeCreateSteamPipe(IntPtr self);

    public int CreateSteamPipe()
    {
        return Call<int, NativeCreateSteamPipe>(Functions.CreateSteamPipe, ObjectAddress);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeReleaseSteamPipe(IntPtr self, int pipe);

    public bool ReleaseSteamPipe(int pipe)
    {
        return Call<bool, NativeReleaseSteamPipe>(Functions.ReleaseSteamPipe, ObjectAddress, pipe);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int NativeConnectToGlobalUser(IntPtr self, int pipe);

    public int ConnectToGlobalUser(int pipe)
    {
        return Call<int, NativeConnectToGlobalUser>(
            Functions.ConnectToGlobalUser,
            ObjectAddress,
            pipe
        );
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate void NativeReleaseUser(IntPtr self, int pipe, int user);

    public void ReleaseUser(int pipe, int user)
    {
        Call<NativeReleaseUser>(Functions.ReleaseUser, ObjectAddress, pipe, user);
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetISteamUser(IntPtr self, int user, int pipe, IntPtr version);

    private TClass GetISteamUser<TClass>(int user, int pipe, string version)
        where TClass : INativeWrapper, new()
    {
        using var nativeVersion = NativeStrings.StringToStringHandle(version);
        IntPtr address = Call<IntPtr, NativeGetISteamUser>(
            Functions.GetISteamUser,
            ObjectAddress,
            user,
            pipe,
            nativeVersion.Handle
        );
        TClass result = new();
        result.SetupFunctions(address);
        return result;
    }

    public SteamUser012 GetSteamUser012(int user, int pipe)
    {
        return GetISteamUser<SteamUser012>(user, pipe, "SteamUser012");
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetISteamUserStats(
        IntPtr self,
        int user,
        int pipe,
        IntPtr version
    );

    private TClass GetISteamUserStats<TClass>(int user, int pipe, string version)
        where TClass : INativeWrapper, new()
    {
        using var nativeVersion = NativeStrings.StringToStringHandle(version);
        IntPtr address = Call<IntPtr, NativeGetISteamUserStats>(
            Functions.GetISteamUserStats,
            ObjectAddress,
            user,
            pipe,
            nativeVersion.Handle
        );
        TClass result = new();
        result.SetupFunctions(address);
        return result;
    }

    public SteamUserStats013 GetSteamUserStats013(int user, int pipe)
    {
        return GetISteamUserStats<SteamUserStats013>(
            user,
            pipe,
            "STEAMUSERSTATS_INTERFACE_VERSION013"
        );
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetISteamUtils(IntPtr self, int pipe, IntPtr version);

    public TClass GetISteamUtils<TClass>(int pipe, string version)
        where TClass : INativeWrapper, new()
    {
        using var nativeVersion = NativeStrings.StringToStringHandle(version);
        IntPtr address = Call<IntPtr, NativeGetISteamUtils>(
            Functions.GetISteamUtils,
            ObjectAddress,
            pipe,
            nativeVersion.Handle
        );
        TClass result = new();
        result.SetupFunctions(address);
        return result;
    }

    public SteamUtils005 GetSteamUtils004(int pipe)
    {
        return GetISteamUtils<SteamUtils005>(pipe, "SteamUtils005");
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate IntPtr NativeGetISteamApps(IntPtr self, int user, int pipe, IntPtr version);

    private TClass GetISteamApps<TClass>(int user, int pipe, string version)
        where TClass : INativeWrapper, new()
    {
        using var nativeVersion = NativeStrings.StringToStringHandle(version);
        IntPtr address = Call<IntPtr, NativeGetISteamApps>(
            Functions.GetISteamApps,
            ObjectAddress,
            user,
            pipe,
            nativeVersion.Handle
        );
        TClass result = new();
        result.SetupFunctions(address);
        return result;
    }

    public SteamApps001 GetSteamApps001(int user, int pipe)
    {
        return GetISteamApps<SteamApps001>(user, pipe, "STEAMAPPS_INTERFACE_VERSION001");
    }

    public SteamApps008 GetSteamApps008(int user, int pipe)
    {
        return GetISteamApps<SteamApps008>(user, pipe, "STEAMAPPS_INTERFACE_VERSION008");
    }
}
