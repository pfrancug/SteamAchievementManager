using SAM.Backend.Hubs;
using SAM.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Parse --appid=N and --auto from command line
long? startupAppId = null;
var autoMode = false;
foreach (var arg in args)
{
    if (arg.StartsWith("--appid=") && long.TryParse(arg["--appid=".Length..], out var id) && id > 0)
    {
        startupAppId = id;
    }
    else if (arg == "--auto")
    {
        autoMode = true;
    }
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Loopback, 0);
});

builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SteamClientService>();
builder.Services.AddSingleton<StatsService>();
builder.Services.AddSingleton<GameLibraryService>();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Backend only binds to 127.0.0.1 (loopback), so allowing any origin is safe.
        // This is required for the packaged Electron build where the renderer loads from
        // a local file (null origin) rather than http://localhost:5173.
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();
app.MapHealthChecks("/health");
app.MapHub<AchievementHub>("/hub");

await app.StartAsync();

// Print PORT so Electron can open the window
foreach (var address in app.Urls)
{
    Console.WriteLine($"PORT:{new Uri(address).Port}");
}

if (startupAppId is not null)
{
    var steam = app.Services.GetRequiredService<SteamClientService>();
    var stats = app.Services.GetRequiredService<StatsService>();
    try
    {
        steam.Connect(startupAppId.Value);
        stats.RequestUserStats();

        if (autoMode)
        {
            await steam.WaitForStatsAsync();
            var achievements = stats.GetAchievements();
            var toUnlock = achievements
                .Where(a => !a.IsProtected && !a.IsUnlocked)
                .Select(a => a.Id)
                .ToArray();
            foreach (var name in toUnlock)
            {
                stats.SetAchievement(name, true);
            }
            stats.StoreStats();
            await app.StopAsync();
            return;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Auto-connect to game {startupAppId.Value} failed: {ex.Message}");
    }
}

await app.WaitForShutdownAsync();
