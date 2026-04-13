using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SAM.Backend.Steam;

public static class SteamNative
{
    [DllImport(
        "kernel32.dll",
        SetLastError = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true
    )]
    private static extern IntPtr GetProcAddress(IntPtr module, string name);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryEx(string path, IntPtr file, uint flags);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string path);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr module);

    private const uint LoadWithAlteredSearchPath = 8;

    private static TDelegate? GetExportFunction<TDelegate>(IntPtr module, string name)
        where TDelegate : Delegate
    {
        IntPtr address = GetProcAddress(module, name);
        return address == IntPtr.Zero
            ? null
            : Marshal.GetDelegateForFunctionPointer<TDelegate>(address);
    }

    private static IntPtr _handle = IntPtr.Zero;

    /// <summary>
    /// Unload steamclient64.dll so the next <see cref="Load"/> picks up a fresh
    /// Steam session. Required after Steam restarts.
    /// </summary>
    public static void Unload()
    {
        if (_handle != IntPtr.Zero)
        {
            FreeLibrary(_handle);
            _handle = IntPtr.Zero;
        }
        _callCreateInterface = null;
        _callSteamBGetCallback = null;
        _callSteamFreeLastCallback = null;
    }

    public static string? GetInstallPath()
    {
        return Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Valve\Steam", "InstallPath", null)
            as string;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private delegate IntPtr NativeCreateInterface(string version, IntPtr returnCode);

    private static NativeCreateInterface? _callCreateInterface;

    public static TClass? CreateInterface<TClass>(string version)
        where TClass : INativeWrapper, new()
    {
        IntPtr address = _callCreateInterface!(version, IntPtr.Zero);
        if (address == IntPtr.Zero)
        {
            return default;
        }
        TClass instance = new();
        instance.SetupFunctions(address);
        return instance;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeSteamGetCallback(
        int pipe,
        out CallbackMessage message,
        out int call
    );

    private static NativeSteamGetCallback? _callSteamBGetCallback;

    public static bool GetCallback(int pipe, out CallbackMessage message, out int call)
    {
        return _callSteamBGetCallback!(pipe, out message, out call);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool NativeSteamFreeLastCallback(int pipe);

    private static NativeSteamFreeLastCallback? _callSteamFreeLastCallback;

    public static bool FreeLastCallback(int pipe)
    {
        return _callSteamFreeLastCallback!(pipe);
    }

    public static bool Load()
    {
        if (_handle != IntPtr.Zero)
        {
            return true;
        }

        string? path = GetInstallPath();
        if (path is null)
        {
            return false;
        }

        SetDllDirectory(path + ";" + Path.Combine(path, "bin"));
        var dllPath = Path.Combine(path, "steamclient64.dll");
        IntPtr module = LoadLibraryEx(dllPath, IntPtr.Zero, LoadWithAlteredSearchPath);
        if (module == IntPtr.Zero)
        {
            return false;
        }

        _callCreateInterface = GetExportFunction<NativeCreateInterface>(module, "CreateInterface");
        if (_callCreateInterface is null)
        {
            return false;
        }

        _callSteamBGetCallback = GetExportFunction<NativeSteamGetCallback>(
            module,
            "Steam_BGetCallback"
        );
        if (_callSteamBGetCallback is null)
        {
            return false;
        }

        _callSteamFreeLastCallback = GetExportFunction<NativeSteamFreeLastCallback>(
            module,
            "Steam_FreeLastCallback"
        );
        if (_callSteamFreeLastCallback is null)
        {
            return false;
        }

        _handle = module;
        return true;
    }
}
