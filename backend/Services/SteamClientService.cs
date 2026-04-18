using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SAM.Backend.Hubs;

namespace SAM.Backend.Services;

public class SteamClientService : IDisposable
{
    private const int StatsTimeoutSeconds = 30;

    private readonly IHubContext<AchievementHub> _hubContext;
    private Steam.SteamClient? _client;
    private Steam.SteamClient? _browserClient;
    private Steam.UserStatsReceivedCallback? _userStatsReceivedCallback;
    private Timer? _callbackTimer;
    private readonly Timer _healthTimer;
    private long _currentAppId;
    private volatile bool _steamDisconnected;

    public event Action<Steam.UserStatsReceivedData>? UserStatsReceived;
    public bool IsConnected => _client is not null && !_steamDisconnected;
    public ulong SteamId
    {
        get
        {
            lock (NativeLock)
            {
                return _client?.SteamUser?.GetSteamId() ?? 0;
            }
        }
    }
    public string CurrentGameName { get; private set; } = "";
    public long CurrentAppId => _currentAppId;
    public Steam.SteamClient? Client => _client;
    public Lock NativeLock { get; } = new();
    private TaskCompletionSource? _statsReady;

    public SteamClientService(IHubContext<AchievementHub> hubContext)
    {
        _hubContext = hubContext;
        _healthTimer = new Timer(
            _ => CheckSteamHealth(),
            null,
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(5)
        );
    }

    private Task BroadcastStatus(string? error = null) =>
        _hubContext.Clients.All.SendAsync(
            "steamStatus",
            new
            {
                IsConnected = IsConnected,
                SteamId = IsConnected ? SteamId.ToString() : (string?)null,
                GameName = IsConnected ? CurrentGameName : (string?)null,
                Error = error,
            }
        );

    public Steam.SteamClient EnsureBrowserClient()
    {
        lock (NativeLock)
        {
            return EnsureBrowserClientCore();
        }
    }

    /// <summary>Must be called while holding <see cref="NativeLock"/>.</summary>
    internal Steam.SteamClient EnsureBrowserClientCore()
    {
        if (_browserClient is not null)
        {
            return _browserClient;
        }
        var client = new Steam.SteamClient();
        try
        {
            client.Initialize(0);
        }
        catch
        {
            client.Dispose();
            throw;
        }
        _browserClient = client;
        return _browserClient;
    }

    public void Connect(long appId)
    {
        lock (NativeLock)
        {
            if (_client is not null)
            {
                Disconnect();
            }

            _browserClient?.Dispose();
            _browserClient = null;

            _client = new Steam.SteamClient();
            try
            {
                _client.Initialize(appId);
            }
            catch (Exception ex)
            {
                _client.Dispose();
                _client = null;
                _ = BroadcastStatus(ex.Message);
                throw;
            }

            _currentAppId = appId;
            _steamDisconnected = false;

            _statsReady = new TaskCompletionSource(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

            _userStatsReceivedCallback =
                _client.CreateAndRegisterCallback<Steam.UserStatsReceivedCallback>();
            _userStatsReceivedCallback.OnRun += data =>
            {
                _statsReady?.TrySetResult();
                UserStatsReceived?.Invoke(data);
            };

            var appDataCallback = _client.CreateAndRegisterCallback<Steam.AppDataChangedCallback>();
            appDataCallback.OnRun += data =>
            {
                if (data.Id != (uint)_currentAppId)
                {
                    return;
                }
                _statsReady = new TaskCompletionSource(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );
                // Already inside NativeLock (called from RunCallbacks)
                _client.SteamUserStats?.RequestUserStats(_client.SteamUser!.GetSteamId());
                _ = BroadcastStatus();
                _ = _hubContext.Clients.All.SendAsync("dataChanged");
            };

            _callbackTimer = new Timer(
                _ => RunCallbacks(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMilliseconds(100)
            );

            CurrentGameName =
                _client.SteamApps001?.GetAppData((uint)appId, "name") ?? $"App {appId}";
        }

        _ = BroadcastStatus();
    }

    /// <summary>Wait for user stats callback (up to <see cref="StatsTimeoutSeconds"/>s).</summary>
    public async Task WaitForStatsAsync()
    {
        var tcs = _statsReady;
        if (tcs is null)
        {
            return;
        }
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(StatsTimeoutSeconds));
        try
        {
            await tcs.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Timed out waiting for user stats callback
        }
    }

    public void Disconnect()
    {
        lock (NativeLock)
        {
            _callbackTimer?.Dispose();
            _callbackTimer = null;
            _statsReady?.TrySetCanceled();
            _statsReady = null;
            _client?.Dispose();
            _client = null;
            _currentAppId = 0;
            _steamDisconnected = false;
            CurrentGameName = "";
        }
        _ = BroadcastStatus();
    }

    public bool Ping()
    {
        try
        {
            lock (NativeLock)
            {
                if (_client is not null)
                {
                    var alive = IsSteamAliveCore(_client);
                    if (alive && _steamDisconnected)
                    {
                        _steamDisconnected = false;
                        _ = BroadcastStatus();
                    }
                    return alive;
                }
                // Browsing mode: try browser client
                var client = EnsureBrowserClientCore();
                var isAlive = IsSteamAliveCore(client);
                if (isAlive && _steamDisconnected)
                {
                    _steamDisconnected = false;
                    _ = BroadcastStatus();
                }
                return isAlive;
            }
        }
        catch
        {
            return false;
        }
    }

    private void RunCallbacks()
    {
        if (_steamDisconnected)
        {
            return;
        }

        try
        {
            lock (NativeLock)
            {
                _client?.RunCallbacks(false);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Steam callback pump failed: {ex.Message}");
            if (_client is not null && !_steamDisconnected)
            {
                _steamDisconnected = true;
                _ = BroadcastStatus("Steam session ended");
            }
        }
    }

    private void CheckSteamHealth()
    {
        try
        {
            lock (NativeLock)
            {
                if (_steamDisconnected)
                {
                    if (!IsSteamProcessRunning())
                    {
                        return;
                    }

                    if (_client is not null)
                    {
                        return;
                    }

                    if (TryReconnectBrowserClientCore())
                    {
                        _steamDisconnected = false;
                        _ = BroadcastStatus();
                    }

                    return;
                }

                if (_client is not null)
                {
                    if (!IsSteamAliveCore(_client))
                    {
                        _steamDisconnected = true;
                        _ = BroadcastStatus("Steam session ended");
                    }
                }
                else if (_browserClient is not null)
                {
                    if (!IsSteamAliveCore(_browserClient))
                    {
                        _steamDisconnected = true;
                        _browserClient.Dispose();
                        _browserClient = null;
                        _ = BroadcastStatus("Steam session ended");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Steam health check error: {ex.Message}");
        }
    }

    /// <summary>
    /// Try to create a fresh browser client and verify Steam is logged in.
    /// Returns false if Steam isn't ready yet (still starting up).
    /// </summary>
    /// <summary>Must be called while holding <see cref="NativeLock"/>.</summary>
    private bool TryReconnectBrowserClientCore()
    {
        try
        {
            _browserClient?.Dispose();
            _browserClient = null;

            Steam.SteamNative.Unload();

            var client = new Steam.SteamClient();
            try
            {
                client.Initialize(0);
            }
            catch
            {
                client.Dispose();
                return false;
            }

            if (client.SteamUser?.IsLoggedIn() != true)
            {
                client.Dispose();
                return false;
            }

            _browserClient = client;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Must be called while holding <see cref="NativeLock"/>.</summary>
    private bool IsSteamAliveCore(Steam.SteamClient client)
    {
        try
        {
            if (!IsSteamProcessRunning())
            {
                return false;
            }
            client.RunCallbacks(false);
            return client.SteamUser?.IsLoggedIn() ?? false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Steam native call failed: {ex.Message}");
            return false;
        }
    }

    private static bool IsSteamProcessRunning()
    {
        try
        {
            return Process.GetProcessesByName("steam").Length > 0;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _healthTimer.Dispose();
        Disconnect();
        lock (NativeLock)
        {
            _browserClient?.Dispose();
            _browserClient = null;
        }
        GC.SuppressFinalize(this);
    }
}
