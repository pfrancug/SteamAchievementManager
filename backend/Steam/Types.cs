using System.Runtime.InteropServices;

namespace SAM.Backend.Steam;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CallbackMessage
{
    public int User;
    public int Id;
    public IntPtr ParamPointer;
    public int ParamSize;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UserStatsReceivedData
{
    public ulong GameId;
    public int Result;
    public ulong SteamIdUser;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AppDataChangedData
{
    public uint Id;
    public bool Result;
}

public enum AccountType : int
{
    Invalid = 0,
    Individual = 1,
    Multiset = 2,
    GameServer = 3,
    AnonGameServer = 4,
    Pending = 5,
    ContentServer = 6,
    Clan = 7,
    Chat = 8,
    P2PSuperSeeder = 9,
}

public enum UserStatType
{
    Invalid = 0,
    Integer = 1,
    Int = Integer,
    Float = 2,
    AverageRate = 3,
    Achievements = 4,
    GroupAchievements = 5,
}

public enum ClientInitializeFailure : byte
{
    Unknown = 0,
    GetInstallPath,
    Load,
    CreateSteamClient,
    CreateSteamPipe,
    ConnectToGlobalUser,
    AppIdMismatch,
}

public class ClientInitializeException : Exception
{
    public ClientInitializeFailure Failure { get; }

    public ClientInitializeException(ClientInitializeFailure failure, string message)
        : base(message)
    {
        Failure = failure;
    }
}
