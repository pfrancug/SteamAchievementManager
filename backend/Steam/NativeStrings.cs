using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace SAM.Backend.Steam;

internal static unsafe class NativeStrings
{
    public sealed class StringHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal StringHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        public IntPtr Handle => handle;

        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(handle);
                handle = IntPtr.Zero;
                return true;
            }
            return false;
        }
    }

    public static StringHandle StringToStringHandle(string? value)
    {
        if (value is null)
        {
            return new StringHandle(IntPtr.Zero, true);
        }
        var bytes = Encoding.UTF8.GetBytes(value);
        var p = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, p, bytes.Length);
        ((byte*)p)[bytes.Length] = 0;
        return new StringHandle(p, true);
    }

    public static string? PointerToString(sbyte* bytes)
    {
        if (bytes == null)
        {
            return null;
        }
        if (*bytes == 0)
        {
            return string.Empty;
        }
        int length = 0;
        while (bytes[length] != 0)
        {
            length++;
        }
        return new string(bytes, 0, length, Encoding.UTF8);
    }

    public static string? PointerToString(byte* bytes) => PointerToString((sbyte*)bytes);

    public static string? PointerToString(IntPtr nativeData) =>
        PointerToString((sbyte*)nativeData.ToPointer());

    public static string? PointerToString(IntPtr nativeData, int maxLength)
    {
        if (nativeData == IntPtr.Zero)
        {
            return null;
        }
        var bytes = (sbyte*)nativeData.ToPointer();
        if (*bytes == 0)
        {
            return string.Empty;
        }
        int length = 0;
        while (length < maxLength && bytes[length] != 0)
        {
            length++;
        }
        return new string(bytes, 0, length, Encoding.UTF8);
    }
}
