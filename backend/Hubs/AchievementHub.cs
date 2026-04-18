using Microsoft.AspNetCore.SignalR;

namespace SAM.Backend.Hubs;

public class AchievementHub : Hub
{
    private readonly Services.SteamClientService _steam;
    private readonly Services.StatsService _stats;
    private readonly Services.GameLibraryService _games;

    public AchievementHub(
        Services.SteamClientService steam,
        Services.StatsService stats,
        Services.GameLibraryService games
    )
    {
        _steam = steam;
        _stats = stats;
        _games = games;
    }

    public async Task<List<Services.GameData>> GetOwnedGames()
    {
        return await _games.GetOwnedGamesAsync();
    }

    public Task<bool> Connect(long appId)
    {
        try
        {
            _steam.Connect(appId);
            _stats.RequestUserStats();
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Hub Connect failed for appId {appId}: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<object> GetStatus()
    {
        if (_steam.IsConnected)
        {
            return Task.FromResult<object>(
                new
                {
                    IsConnected = true,
                    SteamId = _steam.SteamId.ToString(),
                    GameName = _steam.CurrentGameName,
                }
            );
        }
        return Task.FromResult<object>(new { IsConnected = false });
    }

    public async Task<List<Services.AchievementData>> GetAchievements()
    {
        await _steam.WaitForStatsAsync();
        return _stats.GetAchievements();
    }

    public async Task<List<Services.StatData>> GetStats()
    {
        await _steam.WaitForStatsAsync();
        return _stats.GetStats();
    }

    public Task<bool> SetAchievement(string name, bool unlocked)
    {
        return Task.FromResult(_stats.SetAchievement(name, unlocked));
    }

    public Task<bool> SetStat(string name, double value, string type)
    {
        switch (type)
        {
            case "int":
                return Task.FromResult(_stats.SetStat(name, (int)value));
            case "float":
            case "rate":
                return Task.FromResult(_stats.SetStat(name, (float)value));
            default:
                return Task.FromResult(false);
        }
    }

    public Task<bool> StoreStats()
    {
        return Task.FromResult(_stats.StoreStats());
    }

    public Task<bool> ResetStats(bool achievementsToo)
    {
        return Task.FromResult(_stats.ResetStats(achievementsToo));
    }

    public Task<int> BulkSetAchievements(string[] names, bool unlocked)
    {
        int count = 0;
        lock (_steam.NativeLock)
        {
            foreach (var name in names)
            {
                if (_steam.Client?.SteamUserStats?.SetAchievement(name, unlocked) == true)
                {
                    count++;
                }
            }
        }
        return Task.FromResult(count);
    }

    public Task<bool> Ping() => Task.FromResult(_steam.Ping());
}
