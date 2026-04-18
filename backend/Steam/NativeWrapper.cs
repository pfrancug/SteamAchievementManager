using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SAM.Backend.Steam;

public abstract class NativeWrapper<TNativeFunctions> : INativeWrapper
    where TNativeFunctions : struct
{
    protected IntPtr ObjectAddress;
    protected TNativeFunctions Functions;

    public void SetupFunctions(IntPtr objectAddress)
    {
        if (objectAddress == IntPtr.Zero)
        {
            throw new ArgumentException("Steam interface pointer is null.", nameof(objectAddress));
        }
        ObjectAddress = objectAddress;
        var iface = Marshal.PtrToStructure<NativeClass>(objectAddress);
        Functions = Marshal.PtrToStructure<TNativeFunctions>(iface.VirtualTable);
    }

    private readonly ConcurrentDictionary<IntPtr, Delegate> _functionCache = new();

    protected Delegate GetDelegate<TDelegate>(IntPtr pointer)
        where TDelegate : Delegate
    {
        return _functionCache.GetOrAdd(
            pointer,
            static p => Marshal.GetDelegateForFunctionPointer<TDelegate>(p)
        );
    }

    protected TDelegate GetFunction<TDelegate>(IntPtr pointer)
        where TDelegate : Delegate
    {
        return (TDelegate)GetDelegate<TDelegate>(pointer);
    }
}
