using System.Runtime.InteropServices;

namespace SAM.Backend.Steam;

public interface ICallback
{
    int Id { get; }
    bool IsServer { get; }
    void Run(IntPtr param);
}

public abstract class Callback<TParameter> : ICallback
    where TParameter : struct
{
    public delegate void CallbackHandler(TParameter arg);

    public event CallbackHandler? OnRun;
    public abstract int Id { get; }
    public abstract bool IsServer { get; }

    public void Run(IntPtr pvParam)
    {
        var data = Marshal.PtrToStructure<TParameter>(pvParam);
        OnRun?.Invoke(data);
    }
}

public class UserStatsReceivedCallback : Callback<UserStatsReceivedData>
{
    public override int Id => 1101;
    public override bool IsServer => false;
}

public class AppDataChangedCallback : Callback<AppDataChangedData>
{
    public override int Id => 1001;
    public override bool IsServer => false;
}
