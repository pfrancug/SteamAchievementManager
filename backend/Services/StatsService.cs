namespace SAM.Backend.Services;

public class StatsService
{
    private readonly SteamClientService _connection;

    public StatsService(SteamClientService connection)
    {
        _connection = connection;
    }

    public List<AchievementData> GetAchievements()
    {
        lock (_connection.NativeLock)
        {
            return GetAchievementsCore();
        }
    }

    private List<AchievementData> GetAchievementsCore()
    {
        var client = _connection.Client;
        var appId = _connection.CurrentAppId;

        if (client?.SteamUserStats is null)
        {
            return [];
        }

        var achievements = new List<AchievementData>();
        var schemaPath = GetSchemaPath(appId);
        var schema = schemaPath is not null ? Steam.KeyValue.LoadAsBinary(schemaPath) : null;
        var statsDef = schema?[appId.ToString()]["stats"];

        if (statsDef?.Children is null)
        {
            return achievements;
        }

        foreach (var stat in statsDef.Children)
        {
            if (stat.Type != Steam.KeyValueType.None)
            {
                continue;
            }

            var statType = ParseStatType(stat);
            if (
                statType
                is not (Steam.UserStatType.Achievements or Steam.UserStatType.GroupAchievements)
            )
            {
                continue;
            }

            var bits = stat["bits"];
            if (bits?.Children is null)
            {
                continue;
            }

            foreach (var bit in bits.Children)
            {
                var id = bit["name"].AsString("");
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                var permission = bit["permission"].AsInteger(0);
                var display = bit["display"];
                var name = display?["name"].AsString("") ?? "";
                var description = display?["desc"].AsString("") ?? "";
                var isHidden = display?["hidden"].AsBoolean(false) ?? false;

                if (
                    !client.SteamUserStats.GetAchievementAndUnlockTime(
                        id,
                        out var isAchieved,
                        out var unlockTime
                    )
                )
                {
                    continue;
                }

                var iconHash = client.SteamUserStats.GetAchievementDisplayAttribute(id, "icon");
                var iconGrayHash = client.SteamUserStats.GetAchievementDisplayAttribute(
                    id,
                    "icon_gray"
                );

                achievements.Add(
                    new AchievementData
                    {
                        Id = id,
                        Name =
                            client.SteamUserStats.GetAchievementDisplayAttribute(id, "name")
                            ?? name,
                        Description =
                            client.SteamUserStats.GetAchievementDisplayAttribute(id, "desc")
                            ?? description,
                        IsUnlocked = isAchieved,
                        UnlockTime =
                            unlockTime > 0
                                ? DateTimeOffset.FromUnixTimeSeconds(unlockTime).UtcDateTime
                                : null,
                        IsProtected = (permission & 3) != 0,
                        IsHidden = isHidden,
                        Permission = permission,
                        IconUrl = !string.IsNullOrEmpty(iconHash)
                            ? $"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{appId}/{iconHash}"
                            : null,
                        IconLockedUrl = !string.IsNullOrEmpty(iconGrayHash)
                            ? $"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{appId}/{iconGrayHash}"
                            : null,
                    }
                );
            }
        }

        return achievements;
    }

    public List<StatData> GetStats()
    {
        lock (_connection.NativeLock)
        {
            return GetStatsCore();
        }
    }

    private List<StatData> GetStatsCore()
    {
        var client = _connection.Client;
        var appId = _connection.CurrentAppId;

        if (client?.SteamUserStats is null)
        {
            return [];
        }

        var stats = new List<StatData>();
        var schemaPath = GetSchemaPath(appId);
        var schema = schemaPath is not null ? Steam.KeyValue.LoadAsBinary(schemaPath) : null;
        var statsDef = schema?[appId.ToString()]["stats"];

        if (statsDef?.Children is null)
        {
            return stats;
        }

        foreach (var stat in statsDef.Children)
        {
            if (stat.Type != Steam.KeyValueType.None)
            {
                continue;
            }

            var statType = ParseStatType(stat);

            if (
                statType
                is not (
                    Steam.UserStatType.Integer
                    or Steam.UserStatType.Float
                    or Steam.UserStatType.AverageRate
                )
            )
            {
                continue;
            }

            var id = stat["name"].AsString("");
            if (string.IsNullOrEmpty(id))
            {
                continue;
            }

            var display = stat["display"];
            var displayName = display?["name"].AsString("") ?? "";
            var permission = stat["permission"].AsInteger(0);
            var isProtected = (permission & 3) != 0;
            var incrementOnly = stat["incrementonly"].AsBoolean(false);

            var extras = new List<string>();
            if (incrementOnly)
            {
                extras.Add("IncrementOnly");
            }
            if ((permission & ~3) != 0)
            {
                extras.Add("UnknownPermission");
            }
            var extra = string.Join(", ", extras);

            if (statType == Steam.UserStatType.Integer)
            {
                if (client.SteamUserStats.GetStatValue(id, out int intValue))
                {
                    stats.Add(
                        new StatData
                        {
                            Id = id,
                            Name = displayName,
                            Value = intValue,
                            Type = "int",
                            IsProtected = isProtected,
                            MinValue = stat["min"].AsInteger(int.MinValue),
                            MaxValue = stat["max"].AsInteger(int.MaxValue),
                            MaxChange = stat["maxchange"].AsInteger(0),
                            IncrementOnly = incrementOnly,
                            DefaultValue = stat["default"].AsInteger(0),
                            Extra = extra,
                        }
                    );
                }
            }
            else
            {
                // Float and AverageRate both read as float; type string distinguishes them
                var type = statType == Steam.UserStatType.AverageRate ? "rate" : "float";
                if (client.SteamUserStats.GetStatValue(id, out float floatValue))
                {
                    stats.Add(
                        new StatData
                        {
                            Id = id,
                            Name = displayName,
                            Value = floatValue,
                            Type = type,
                            IsProtected = isProtected,
                            MinValue = stat["min"].AsFloat(float.MinValue),
                            MaxValue = stat["max"].AsFloat(float.MaxValue),
                            MaxChange = stat["maxchange"].AsFloat(0),
                            IncrementOnly = incrementOnly,
                            DefaultValue = stat["default"].AsFloat(0),
                            Extra = extra,
                        }
                    );
                }
            }
        }

        return stats;
    }

    public bool SetAchievement(string name, bool unlocked)
    {
        lock (_connection.NativeLock)
        {
            return _connection.Client?.SteamUserStats?.SetAchievement(name, unlocked) ?? false;
        }
    }

    public bool SetStat(string name, int value)
    {
        lock (_connection.NativeLock)
        {
            return _connection.Client?.SteamUserStats?.SetStatValue(name, value) ?? false;
        }
    }

    public bool SetStat(string name, float value)
    {
        lock (_connection.NativeLock)
        {
            return _connection.Client?.SteamUserStats?.SetStatValue(name, value) ?? false;
        }
    }

    public bool StoreStats()
    {
        lock (_connection.NativeLock)
        {
            return _connection.Client?.SteamUserStats?.StoreStats() ?? false;
        }
    }

    public bool ResetStats(bool achievementsToo)
    {
        lock (_connection.NativeLock)
        {
            return _connection.Client?.SteamUserStats?.ResetAllStats(achievementsToo) ?? false;
        }
    }

    public void RequestUserStats()
    {
        lock (_connection.NativeLock)
        {
            var client = _connection.Client;
            if (client?.SteamUser is not null && client.SteamUserStats is not null)
            {
                client.SteamUserStats.RequestUserStats(client.SteamUser.GetSteamId());
            }
        }
    }

    private static Steam.UserStatType ParseStatType(Steam.KeyValue stat)
    {
        // New schema format: "type" is a string like "Int", "Float", "Achievements"
        var typeNode = stat["type"];
        if (
            typeNode.Valid
            && typeNode.Type is Steam.KeyValueType.String or Steam.KeyValueType.WideString
        )
        {
            if (Enum.TryParse<Steam.UserStatType>((string)typeNode.Value!, true, out var parsed))
            {
                return parsed;
            }
        }

        // Old schema format: "type_int" is an integer, fall back to "type" as integer
        var typeIntNode = stat["type_int"];
        var rawType = typeIntNode.Valid ? typeIntNode.AsInteger(0) : typeNode.AsInteger(0);
        return (Steam.UserStatType)rawType;
    }

    private static string? GetSchemaPath(long appId)
    {
        var steamPath = Steam.SteamNative.GetInstallPath();
        if (steamPath is null)
        {
            return null;
        }
        var path = Path.Combine(steamPath, "appcache", "stats", $"UserGameStatsSchema_{appId}.bin");
        return File.Exists(path) ? path : null;
    }
}
