import type { HubConnection } from '@microsoft/signalr';
import type {
  AchievementInfo,
  GameInfo,
  StatInfo,
  SteamStatus,
} from '@sam/shared';

import { HubConnectionState } from '@microsoft/signalr';
import { createHubConnection, getBackendPort } from '@services/signalr';
import {
  type ReactNode,
  useCallback,
  useEffect,
  useRef,
  useState,
} from 'react';

import { SteamHubContext } from './SteamHubContext';

export const SteamHubProvider = ({ children }: { children: ReactNode }) => {
  const [status, setStatus] = useState<SteamStatus>({ isConnected: false });
  const [games, setGames] = useState<GameInfo[]>([]);
  const [gamesLoading, setGamesLoading] = useState(false);
  const [achievements, setAchievements] = useState<AchievementInfo[]>([]);
  const [stats, setStats] = useState<StatInfo[]>([]);
  const [isConnecting, setIsConnecting] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [hubReady, setHubReady] = useState(false);
  const hubRef = useRef<HubConnection | null>(null);
  const connectAbortRef = useRef<AbortController | null>(null);
  const modifiedAchievementIdsRef = useRef(new Set<string>());
  const modifiedStatIdsRef = useRef(new Set<string>());
  const loadGamesRef = useRef<() => Promise<void>>(() => Promise.resolve());

  const stopHub = async () => {
    if (hubRef.current) {
      try {
        await hubRef.current.stop();
      } catch {
        // ignore — connection may already be closing
      }
      hubRef.current = null;
    }
  };

  const createHub = async (port: number): Promise<HubConnection> => {
    await stopHub();

    const hub = createHubConnection(port);
    hub.on('steamStatus', (s: SteamStatus) => {
      setStatus((prev) => {
        // Auto-reload games when recovering from an error
        if (prev.error && !s.error) {
          void loadGamesRef.current();
        }

        return s;
      });
    });
    hub.onreconnecting(() =>
      setStatus((prev) => ({ ...prev, isConnected: false })),
    );
    await hub.start();
    hubRef.current = hub;

    return hub;
  };

  // Initial connection to browsing backend
  useEffect(() => {
    let cancelled = false;

    const init = async () => {
      const port = await getBackendPort();
      const hub = await createHub(port);
      if (!cancelled) {
        try {
          const alive = await hub.invoke<boolean>('Ping');
          if (!alive) {
            setStatus({ isConnected: false, error: 'Steam is not running' });
          }
        } catch {
          setStatus({ isConnected: false, error: 'Steam is not running' });
        }
        setHubReady(true);
      }
    };

    init().catch(console.error);

    return () => {
      cancelled = true;
      stopHub();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const loadGames = useCallback(async () => {
    if (!hubRef.current) {
      return;
    }
    const hub = hubRef.current;

    // Reconnect if the hub was dropped (e.g. backend briefly restarted)
    if (hub.state === HubConnectionState.Disconnected) {
      await hub.start();
    }

    setGamesLoading(true);
    try {
      const list = await hub.invoke<GameInfo[]>('GetOwnedGames');
      setGames(list);
      setStatus((prev) => (prev.error ? { ...prev, error: undefined } : prev));
    } catch (err) {
      setStatus((prev) => ({
        ...prev,
        error: err instanceof Error ? err.message : 'Failed to load games',
      }));
      throw err;
    } finally {
      setGamesLoading(false);
    }
  }, []);

  loadGamesRef.current = loadGames;

  const connectToGame = useCallback(
    async (appId: number) => {
      // Electron multi-window: fire-and-forget — the new window handles its own loading
      if (window.electronAPI) {
        const game = games.find((g) => g.appId === appId);
        void window.electronAPI.openGameWindow(
          appId,
          game?.name ?? `App ${appId}`,
          game?.imageUrl,
          game?.purchaseTimestamp,
          game?.type,
        );

        return;
      }

      // Dev mode: single-window fallback
      connectAbortRef.current?.abort();
      const abort = new AbortController();
      connectAbortRef.current = abort;

      setIsConnecting(true);
      setAchievements([]);
      setStats([]);
      modifiedAchievementIdsRef.current.clear();
      modifiedStatIdsRef.current.clear();
      try {
        await stopHub();
        if (abort.signal.aborted) {
          return;
        }

        const port = await getBackendPort();
        if (abort.signal.aborted) {
          return;
        }

        const hub = await createHub(port);
        if (abort.signal.aborted) {
          return;
        }

        const gameStatus = await hub.invoke<SteamStatus>('GetStatus');
        if (abort.signal.aborted) {
          return;
        }

        setStatus(gameStatus);
        if (!gameStatus.isConnected) {
          throw new Error(gameStatus.error ?? 'Failed to connect to game');
        }

        setIsConnecting(false);
        setIsLoading(true);
        const [achs, sts] = await Promise.all([
          hub.invoke<AchievementInfo[]>('GetAchievements'),
          hub.invoke<StatInfo[]>('GetStats'),
        ]);
        if (abort.signal.aborted) {
          return;
        }

        setAchievements(achs);
        setStats(sts);
      } catch (err) {
        if (abort.signal.aborted) {
          return;
        }
        throw err;
      } finally {
        if (!abort.signal.aborted) {
          setIsConnecting(false);
          setIsLoading(false);
        }
      }
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [games],
  );

  const disconnectFromGame = useCallback(async () => {
    // Cancel any in-flight connection
    connectAbortRef.current?.abort();
    connectAbortRef.current = null;

    setAchievements([]);
    setStats([]);
    setIsConnecting(false);
    setIsLoading(false);
    modifiedAchievementIdsRef.current.clear();
    modifiedStatIdsRef.current.clear();

    await stopHub();

    // Reconnect to browsing backend
    const port = await getBackendPort();
    await createHub(port);
    setStatus({ isConnected: false });
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
    // Clear error immediately so the banner disappears, then load in the background
    setStatus((prev) => ({ ...prev, error: undefined }));
    void loadGames();
  }, [loadGames]);

  return (
    <SteamHubContext
      value={{
        status,
        games,
        gamesLoading,
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
