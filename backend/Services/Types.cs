namespace SAM.Backend.Services;

public class GameData
{
    public required uint AppId { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? ImageUrl { get; init; }
    public uint? PurchaseTimestamp { get; init; }
}

public class AchievementData
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public bool IsUnlocked { get; init; }
    public DateTime? UnlockTime { get; init; }
    public bool IsProtected { get; init; }
    public bool IsHidden { get; init; }
    public int Permission { get; init; }
    public string? IconUrl { get; init; }
    public string? IconLockedUrl { get; init; }
}

public class StatData
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required object Value { get; init; }
    public required string Type { get; init; }
    public bool IsProtected { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public double? MaxChange { get; init; }
    public bool IncrementOnly { get; init; }
    public double? DefaultValue { get; init; }
    public string Extra { get; init; } = "";
}
