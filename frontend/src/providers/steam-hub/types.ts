import type {
  AchievementInfo,
  GameInfo,
  StatInfo,
  SteamStatus,
} from '@sam/shared';

export interface SteamHubState {
  status: SteamStatus;
  games: GameInfo[];
  gamesLoading: boolean;
  achievements: AchievementInfo[];
  stats: StatInfo[];
  isConnecting: boolean;
  isLoading: boolean;
  hubReady: boolean;
}

export interface SteamHubActions {
  loadGames: () => Promise<void>;
  connectToGame: (appId: number) => Promise<void>;
  disconnectFromGame: () => Promise<void>;
  setAchievement: (name: string, unlocked: boolean) => Promise<boolean>;
  setStat: (
    name: string,
    value: number,
    type: 'int' | 'float' | 'rate',
  ) => Promise<boolean>;
  storeStats: () => Promise<boolean>;
  resetStats: (achievementsToo: boolean) => Promise<boolean>;
  bulkSetAchievements: (names: string[], unlocked: boolean) => Promise<number>;
  refreshAchievements: () => Promise<void>;
  refreshStats: () => Promise<void>;
  retryConnect: () => Promise<void>;
  getModifiedCounts: () => { achievements: number; stats: number };
}

export type SteamHubContextValue = SteamHubState & SteamHubActions;
