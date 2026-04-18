using System.Runtime.InteropServices;
using SAM.Backend.Steam.Interfaces;

namespace SAM.Backend.Steam.Wrappers;

public class SteamApps001 : NativeWrapper<ISteamApps001>
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate int NativeGetAppData(
        IntPtr self,
        uint appId,
        IntPtr key,
        IntPtr value,
        int valueLength
    );

    public string? GetAppData(uint appId, string key)
    {
        using var nativeHandle = NativeStrings.StringToStringHandle(key);
        const int valueLength = 1024;
        var valuePointer = Marshal.AllocHGlobal(valueLength);
        try
        {
            var call = GetFunction<NativeGetAppData>(Functions.GetAppData);
            int result = call(ObjectAddress, appId, nativeHandle.Handle, valuePointer, valueLength);
            return result == 0 ? null : NativeStrings.PointerToString(valuePointer, valueLength);
        }
        finally
        {
            Marshal.FreeHGlobal(valuePointer);
        }
    }
}
