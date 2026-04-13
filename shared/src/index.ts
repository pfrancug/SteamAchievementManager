export type GameType = 'normal' | 'demo' | 'mod' | 'junk';

export interface GameInfo {
  appId: number;
  name: string;
  type: GameType;
  imageUrl?: string;
  purchaseTimestamp?: number;
}

export interface AchievementInfo {
  id: string;
  name: string;
  description: string;
  isUnlocked: boolean;
  isProtected: boolean;
  isHidden: boolean;
  permission: number;
  unlockTime?: string;
  iconUrl?: string;
  iconLockedUrl?: string;
}

export interface StatInfo {
  id: string;
  name: string;
  value: number;
  type: 'int' | 'float' | 'rate';
  isProtected: boolean;
  minValue?: number;
  maxValue?: number;
  maxChange?: number;
  incrementOnly: boolean;
  defaultValue?: number;
  extra: string;
}

export interface SteamStatus {
  isConnected: boolean;
  steamId?: string;
  gameName?: string;
  error?: string;
}

// SignalR hub method names (server -> client)
export interface ServerToClientEvents {
  steamStatus: (status: SteamStatus) => void;
  dataChanged: () => void;
}

// SignalR hub method names (client -> server)
export interface ClientToServerEvents {
  getOwnedGames: () => Promise<GameInfo[]>;
  connect: (appId: number) => Promise<boolean>;
  getStatus: () => Promise<SteamStatus>;
  getAchievements: () => Promise<AchievementInfo[]>;
  getStats: () => Promise<StatInfo[]>;
  ping: () => Promise<boolean>;
  setAchievement: (name: string, unlocked: boolean) => Promise<boolean>;
  setStat: (name: string, value: number, type: string) => Promise<boolean>;
  storeStats: () => Promise<boolean>;
  resetStats: (achievementsToo: boolean) => Promise<boolean>;
  bulkSetAchievements: (names: string[], unlocked: boolean) => Promise<number>;
}
