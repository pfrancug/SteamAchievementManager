import type { GameHubProviderProps } from './types';
import type {
  AchievementInfo,
  GameInfo,
  StatInfo,
  SteamStatus,
} from '@sam/shared';

import { createHubConnection, type HubConnection } from '@services/signalr';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';

import { SteamHubContext } from '../steam-hub/SteamHubContext';

export const GameHubProvider = ({
  children,
  port,
  appId,
  gameName,
  gameType,
  imageUrl,
  purchaseTimestamp,
}: GameHubProviderProps) => {
  const [status, setStatus] = useState<SteamStatus>({ isConnected: false });
  const [achievements, setAchievements] = useState<AchievementInfo[]>([]);
  const [stats, setStats] = useState<StatInfo[]>([]);
  const [isConnecting, setIsConnecting] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [hubReady, setHubReady] = useState(false);

  const hubRef = useRef<HubConnection | null>(null);
  const modifiedAchievementIdsRef = useRef(new Set<string>());
  const modifiedStatIdsRef = useRef(new Set<string>());

  useEffect(() => {
    let cancelled = false;

    const init = async () => {
      const hub = createHubConnection(port);
      hub.on('steamStatus', (s: SteamStatus) => {
        if (s.error && window.electronAPI) {
          void window.electronAPI.closeWindow();

          return;
        }
        setStatus(s);
      });
      hub.on('dataChanged', () => {
        hub
          .invoke<AchievementInfo[]>('GetAchievements')
          .then(setAchievements)
          .catch(console.error);
        hub.invoke<StatInfo[]>('GetStats').then(setStats).catch(console.error);
      });
      hub.onreconnecting(() =>
        setStatus((prev) => ({ ...prev, isConnected: false })),
      );
      await hub.start();
      hubRef.current = hub;

      if (cancelled) {
        await hub.stop();

        return;
      }

      setHubReady(true);

      const gameStatus = await hub.invoke<SteamStatus>('GetStatus');
      if (cancelled) {
        return;
      }

      if (!gameStatus.isConnected) {
        setStatus({
          ...gameStatus,
          error: gameStatus.error ?? 'Steam is not connected',
        });
        setIsConnecting(false);

        return;
      }
      setStatus(gameStatus);

      setIsConnecting(false);
      setIsLoading(true);

      const [achs, sts] = await Promise.all([
        hub.invoke<AchievementInfo[]>('GetAchievements'),
        hub.invoke<StatInfo[]>('GetStats'),
      ]);
      if (cancelled) {
        return;
      }

      setAchievements(achs);
      setStats(sts);
      setIsLoading(false);
    };

    init().catch((err) => {
      console.error('GameHubProvider init failed:', err);
      if (!cancelled) {
        setIsConnecting(false);
        setIsLoading(false);
      }
    });

    return () => {
      cancelled = true;
      if (hubRef.current) {
        hubRef.current.stop().catch(() => {});
        hubRef.current = null;
      }
    };
  }, [port]);

  const loadGames = useCallback(async () => {
    // No-op in game window — games list not needed
  }, []);

  const connectToGame = useCallback(async () => {
    // No-op — game window is already connected
  }, []);

  const disconnectFromGame = useCallback(async () => {
    window.close();
  }, []);

  const setAchievement = useCallback(
    async (name: string, unlocked: boolean): Promise<boolean> => {
      if (!hubRef.current) {
        return false;
      }
      const ok = await hubRef.current.invoke<boolean>(
        'SetAchievement',
        name,
        unlocked,
      );
      if (ok) {
        modifiedAchievementIdsRef.current.add(name);
      }

      return ok;
    },
    [],
  );

  const setStat = useCallback(
    async (
      name: string,
      value: number,
      type: 'int' | 'float' | 'rate',
    ): Promise<boolean> => {
      if (!hubRef.current) {
        return false;
      }
      const ok = await hubRef.current.invoke<boolean>(
        'SetStat',
        name,
        value,
        type,
      );
      if (ok) {
        modifiedStatIdsRef.current.add(name);
      }

      return ok;
    },
    [],
  );

  const storeStats = useCallback(async (): Promise<boolean> => {
    if (!hubRef.current) {
      return false;
    }
    const ok = await hubRef.current.invoke<boolean>('StoreStats');
    if (ok) {
      modifiedAchievementIdsRef.current.clear();
      modifiedStatIdsRef.current.clear();
    }

    return ok;
  }, []);

  const resetStats = useCallback(
    async (achievementsToo: boolean): Promise<boolean> => {
      if (!hubRef.current) {
        return false;
      }
      const ok = await hubRef.current.invoke<boolean>(
        'ResetStats',
        achievementsToo,
      );
      if (ok) {
        modifiedAchievementIdsRef.current.clear();
        modifiedStatIdsRef.current.clear();
      }

      return ok;
    },
    [],
  );

  const bulkSetAchievements = useCallback(
    async (names: string[], unlocked: boolean): Promise<number> => {
      if (!hubRef.current) {
        return 0;
      }
      const count = await hubRef.current.invoke<number>(
        'BulkSetAchievements',
        names,
        unlocked,
      );
      if (count > 0) {
        names.forEach((n) => modifiedAchievementIdsRef.current.add(n));
      }

      return count;
    },
    [],
  );

  const refreshAchievements = useCallback(async () => {
    if (!hubRef.current) {
      return;
    }
    const achs =
      await hubRef.current.invoke<AchievementInfo[]>('GetAchievements');
    setAchievements(achs);
  }, []);

  const refreshStats = useCallback(async () => {
    if (!hubRef.current) {
      return;
    }
    const sts = await hubRef.current.invoke<StatInfo[]>('GetStats');
    setStats(sts);
  }, []);

  const getModifiedCounts = useCallback(
    () => ({
      achievements: modifiedAchievementIdsRef.current.size,
      stats: modifiedStatIdsRef.current.size,
    }),
    [],
  );

  const retryConnect = useCallback(async () => {
    if (!hubRef.current) {
      return;
    }
    const alive = await hubRef.current.invoke<boolean>('Ping');
    if (!alive) {
      throw new Error('Steam is not running');
    }
    // Clear error immediately so the banner disappears, then reconnect in the background
    setStatus((prev) => ({ ...prev, error: undefined }));
    setIsLoading(true);
    const hub = hubRef.current;
    const doReconnect = async (): Promise<void> => {
      try {
        const ok = await hub.invoke<boolean>('Connect', appId);
        if (!ok) {
          setStatus((prev) => ({
            ...prev,
            error: prev.error ?? 'Failed to reconnect to game',
          }));

          return;
        }
        const gameStatus = await hub.invoke<SteamStatus>('GetStatus');
        setStatus(gameStatus);
        if (!gameStatus.isConnected) {
          return;
        }
        modifiedAchievementIdsRef.current.clear();
        modifiedStatIdsRef.current.clear();
        const [achs, sts] = await Promise.all([
          hub.invoke<AchievementInfo[]>('GetAchievements'),
          hub.invoke<StatInfo[]>('GetStats'),
        ]);
        setAchievements(achs);
        setStats(sts);
      } catch (err) {
        console.error('retryConnect reconnect failed:', err);
        setStatus((prev) => ({
          ...prev,
          error: err instanceof Error ? err.message : 'Reconnect failed',
        }));
      } finally {
        setIsLoading(false);
      }
    };
    void doReconnect();
  }, [appId]);

  const stubGame = useMemo<GameInfo>(
    () => ({
      appId,
      imageUrl,
      name: gameName,
      purchaseTimestamp,
      type: gameType,
    }),
    [appId, gameName, gameType, imageUrl, purchaseTimestamp],
  );

  return (
    <SteamHubContext
      value={{
        status,
        games: [stubGame],
        gamesLoading: false,
        achievements,
        stats,
        isConnecting,
        isLoading,
        hubReady,
        loadGames,
        connectToGame,
        disconnectFromGame,
        setAchievement,
        setStat,
        storeStats,
        resetStats,
        bulkSetAchievements,
        refreshAchievements,
        refreshStats,
        retryConnect,
        getModifiedCounts,
      }}
    >
      {children}
    </SteamHubContext>
  );
};
