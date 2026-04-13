namespace SAM.Backend.Services;

public class GameLibraryService
{
    private readonly SteamClientService _connection;
    private readonly IHttpClientFactory _httpFactory;

    public GameLibraryService(SteamClientService connection, IHttpClientFactory httpFactory)
    {
        _connection = connection;
        _httpFactory = httpFactory;
    }

    public async Task<List<GameData>> GetOwnedGamesAsync()
    {
        var gameEntries = await DownloadGameListAsync();

        if (gameEntries.Count == 0)
        {
            return [];
        }

        var games = new List<GameData>();

        lock (_connection.NativeLock)
        {
            var browser = _connection.EnsureBrowserClientCore();

            foreach (var (id, type) in gameEntries)
            {
                if (browser.SteamApps008?.IsSubscribedApp(id) != true)
                {
                    continue;
                }

                var name = browser.SteamApps001?.GetAppData(id, "name");
                var imageUrl = GetGameImageUrl(browser, id);
                var purchaseTime = browser.SteamApps008?.GetEarliestPurchaseUnixTime(id) ?? 0;
                games.Add(
                    new GameData
                    {
                        AppId = id,
                        Name = name ?? id.ToString(),
                        Type = type,
                        ImageUrl = imageUrl,
                        PurchaseTimestamp = purchaseTime > 0 ? purchaseTime : null,
                    }
                );
            }
        }

        return games.OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static string? GetGameImageUrl(Steam.SteamClient browser, uint appId)
    {
        // 1. Localized small capsule
        var lang = browser.SteamApps008?.GetCurrentGameLanguage() ?? "english";
        var capsule = browser.SteamApps001?.GetAppData(appId, $"small_capsule/{lang}");
        if (!string.IsNullOrEmpty(capsule))
        {
            return $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appId}/{capsule}";
        }

        // 2. English fallback
        if (lang != "english")
        {
            capsule = browser.SteamApps001?.GetAppData(appId, "small_capsule/english");
            if (!string.IsNullOrEmpty(capsule))
            {
                return $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appId}/{capsule}";
            }
        }

        // 3. Legacy logo hash fallback
        var logo = browser.SteamApps001?.GetAppData(appId, "logo");
        if (!string.IsNullOrEmpty(logo))
        {
            return $"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{appId}/{logo}.jpg";
        }

        return null;
    }

    private async Task<List<(uint id, string type)>> DownloadGameListAsync()
    {
        using var http = _httpFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(30);
        var xml = await http.GetStringAsync("https://gib.me/sam/games.xml");
        var doc = new System.Xml.XPath.XPathDocument(new StringReader(xml));
        var nav = doc.CreateNavigator();
        var nodes = nav.Select("/games/game");
        var list = new List<(uint id, string type)>();
        while (nodes.MoveNext())
        {
            if (uint.TryParse(nodes.Current?.Value, out var id))
            {
                var type = nodes.Current?.GetAttribute("type", "") ?? "";
                if (string.IsNullOrEmpty(type))
                {
                    type = "normal";
                }
                list.Add((id, type));
            }
        }
        return list;
    }
}
