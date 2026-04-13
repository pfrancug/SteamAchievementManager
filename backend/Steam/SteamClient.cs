using System.Globalization;
using SAM.Backend.Steam.Wrappers;

namespace SAM.Backend.Steam;

public class SteamClient : IDisposable
{
    private const string SteamClientVersion = "SteamClient018";

    public SteamClient018? Client { get; private set; }
    public SteamUser012? SteamUser { get; private set; }
    public SteamUserStats013? SteamUserStats { get; private set; }
    public SteamUtils005? SteamUtils { get; private set; }
    public SteamApps001? SteamApps001 { get; private set; }
    public SteamApps008? SteamApps008 { get; private set; }

    private bool _isDisposed;
    private int _pipe;
    private int _user;
    private readonly List<ICallback> _callbacks = [];
    private readonly Lock _callbackLock = new();

    public void Initialize(long appId)
    {
        if (string.IsNullOrEmpty(SteamNative.GetInstallPath()))
        {
            throw new ClientInitializeException(
                ClientInitializeFailure.GetInstallPath,
                "failed to get Steam install path"
            );
        }

        if (appId != 0)
        {
            Environment.SetEnvironmentVariable(
                "SteamAppId",
                appId.ToString(CultureInfo.InvariantCulture)
            );
        }

        if (!SteamNative.Load())
        {
            throw new ClientInitializeException(
                ClientInitializeFailure.Load,
                "failed to load SteamClient"
            );
        }

        Client = SteamNative.CreateInterface<SteamClient018>(SteamClientVersion);
        if (Client is null)
        {
            throw new ClientInitializeException(
                ClientInitializeFailure.CreateSteamClient,
                $"failed to create I{SteamClientVersion}"
            );
        }

        _pipe = Client.CreateSteamPipe();
        if (_pipe == 0)
        {
            throw new ClientInitializeException(
                ClientInitializeFailure.CreateSteamPipe,
                "failed to create pipe"
            );
        }

        _user = Client.ConnectToGlobalUser(_pipe);
        if (_user == 0)
        {
            throw new ClientInitializeException(
                ClientInitializeFailure.ConnectToGlobalUser,
                "failed to connect to global user"
            );
        }

        SteamUtils = Client.GetSteamUtils004(_pipe);
        if (appId > 0 && SteamUtils.GetAppId() != (uint)appId)
        {
            throw new ClientInitializeException(
                ClientInitializeFailure.AppIdMismatch,
                "appID mismatch"
            );
        }

        SteamUser = Client.GetSteamUser012(_user, _pipe);
        SteamUserStats = Client.GetSteamUserStats013(_user, _pipe);
        SteamApps001 = Client.GetSteamApps001(_user, _pipe);
        SteamApps008 = Client.GetSteamApps008(_user, _pipe);
    }

    public TCallback CreateAndRegisterCallback<TCallback>()
        where TCallback : ICallback, new()
    {
        TCallback callback = new();
        _callbacks.Add(callback);
        return callback;
    }

    public void RunCallbacks(bool server)
    {
        lock (_callbackLock)
        {
            while (SteamNative.GetCallback(_pipe, out var message, out _))
            {
                var callbackId = message.Id;
                foreach (
                    var callback in _callbacks.Where(c =>
                        c.Id == callbackId && c.IsServer == server
                    )
                )
                {
                    callback.Run(message.ParamPointer);
                }
                SteamNative.FreeLastCallback(_pipe);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (Client is not null && _pipe > 0)
        {
            if (_user > 0)
            {
                Client.ReleaseUser(_pipe, _user);
                _user = 0;
            }
            Client.ReleaseSteamPipe(_pipe);
            _pipe = 0;
        }

        _isDisposed = true;
    }

    ~SteamClient()
    {
        Dispose(false);
    }
}
