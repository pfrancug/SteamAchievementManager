import type {
  ChartAggregates,
  GameResult,
  GameStatus,
} from '@features/bulk-unlock/types';
import type { AchievementInfo, SteamStatus } from '@sam/shared';
import type { ReactNode } from 'react';

import { CONCURRENCY, FLUSH_INTERVAL } from '@features/bulk-unlock/constants';
import { createHubConnection } from '@services/signalr';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';

import {
  BulkUnlockProgressContext,
  BulkUnlockResultsContext,
} from './BulkUnlockContext';

const EMPTY_AGGREGATES: ChartAggregates = {
  gamesDone: 0,
  gamesFailed: 0,
  gamesSkipped: 0,
  totalAlreadyUnlocked: 0,
  totalProtected: 0,
  totalUnlocked: 0,
};

interface BulkUnlockProviderProps {
  children: ReactNode;
}

export const BulkUnlockProvider = ({ children }: BulkUnlockProviderProps) => {
  const [phase, setPhase] = useState<
    'loading' | 'processing' | 'done' | 'cancelled'
  >('loading');
  const [elapsed, setElapsed] = useState(0);
  const [aggregates, setAggregates] =
    useState<ChartAggregates>(EMPTY_AGGREGATES);
  const [completedGames, setCompletedGames] = useState(0);
  const [totalGames, setTotalGames] = useState(0);
  const [version, setVersion] = useState(0);

  const resultsRef = useRef<GameResult[]>([]);
  const aggregatesRef = useRef<ChartAggregates>({ ...EMPTY_AGGREGATES });
  const completedRef = useRef(0);
  const startTimeRef = useRef(0);
  const cancelledRef = useRef(false);
  const processingRef = useRef(false);
  const dirtyRef = useRef(false);

  // Throttled flush — transfers ref data to state every FLUSH_INTERVAL ms
  useEffect(() => {
    if (phase !== 'processing') {
      return;
    }
    const interval = setInterval(() => {
      setElapsed(Math.floor((Date.now() - startTimeRef.current) / 1000));
      if (dirtyRef.current) {
        setVersion((v) => v + 1);
        setAggregates({ ...aggregatesRef.current });
        setCompletedGames(completedRef.current);
        dirtyRef.current = false;
      }
    }, FLUSH_INTERVAL);

    return () => clearInterval(interval);
  }, [phase]);

  const updateResult = useCallback(
    (index: number, update: Partial<GameResult>) => {
      resultsRef.current[index] = {
        ...resultsRef.current[index],
        ...update,
      };
      dirtyRef.current = true;
    },
    [],
  );

  const completeGame = useCallback(
    (index: number, update: Partial<GameResult>) => {
      resultsRef.current[index] = {
        ...resultsRef.current[index],
        ...update,
      };
      completedRef.current++;

      if (update.status === 'done') {
        aggregatesRef.current = {
          ...aggregatesRef.current,
          gamesDone: aggregatesRef.current.gamesDone + 1,
          totalUnlocked:
            aggregatesRef.current.totalUnlocked + (update.unlocked ?? 0),
          totalAlreadyUnlocked:
            aggregatesRef.current.totalAlreadyUnlocked +
            (update.alreadyUnlocked ?? 0),
          totalProtected:
            aggregatesRef.current.totalProtected + (update.protected ?? 0),
        };
      } else if (update.status === 'failed') {
        aggregatesRef.current = {
          ...aggregatesRef.current,
          gamesFailed: aggregatesRef.current.gamesFailed + 1,
        };
      } else if (update.status === 'skipped') {
        aggregatesRef.current = {
          ...aggregatesRef.current,
          gamesSkipped: aggregatesRef.current.gamesSkipped + 1,
        };
      }

      dirtyRef.current = true;
    },
    [],
  );

  const processGame = useCallback(
    async (
      game: { appId: number; name: string },
      index: number,
    ): Promise<void> => {
      if (!window.electronAPI) {
        return;
      }

      updateResult(index, { status: 'in-progress' as GameStatus });

      let port = 0;
      try {
        port = await window.electronAPI.startTempBackend(game.appId);

        const hub = createHubConnection(port);
        await hub.start();

        const status = await hub.invoke<SteamStatus>('GetStatus');
        if (!status.isConnected) {
          await hub.stop();
          completeGame(index, {
            status: 'failed',
            error: 'Steam not connected',
          });
          await window.electronAPI.stopTempBackend(port);

          return;
        }

        const achievements =
          await hub.invoke<AchievementInfo[]>('GetAchievements');
        const total = achievements.length;
        const protectedCount = achievements.filter((a) => a.isProtected).length;
        const alreadyUnlocked = achievements.filter(
          (a) => a.isUnlocked && !a.isProtected,
        ).length;
        const toUnlock = achievements.filter(
          (a) => !a.isUnlocked && !a.isProtected,
        );

        if (toUnlock.length === 0) {
          await hub.stop();
          completeGame(index, {
            status: 'done',
            unlocked: 0,
            alreadyUnlocked,
            protected: protectedCount,
            total,
          });
          await window.electronAPI.stopTempBackend(port);

          return;
        }

        const unlocked = await hub.invoke<number>(
          'BulkSetAchievements',
          toUnlock.map((a) => a.id),
          true,
        );

        await hub.invoke<boolean>('StoreStats');
        await hub.stop();

        completeGame(index, {
          status: 'done',
          unlocked,
          alreadyUnlocked,
          protected: protectedCount,
          total,
        });
      } catch (err) {
        const msg = err instanceof Error ? err.message : 'Unknown error';
        completeGame(index, { status: 'failed', error: msg });
      } finally {
        if (port > 0 && window.electronAPI) {
          try {
            await window.electronAPI.stopTempBackend(port);
          } catch {
            // Already stopped
          }
        }
      }
    },
    [completeGame, updateResult],
  );

  const finalFlush = useCallback(() => {
    setVersion((v) => v + 1);
    setAggregates({ ...aggregatesRef.current });
    setCompletedGames(completedRef.current);
  }, []);

  const startProcessing = useCallback(async () => {
    if (processingRef.current || !window.electronAPI) {
      return;
    }
    processingRef.current = true;

    const games = await window.electronAPI.getBulkGames();
    const initial: GameResult[] = games.map((g) => ({
      appId: g.appId,
      imageUrl: g.imageUrl,
      name: g.name,
      status: 'pending' as GameStatus,
      unlocked: 0,
      alreadyUnlocked: 0,
      protected: 0,
      total: 0,
    }));

    resultsRef.current = initial;
    setVersion((v) => v + 1);
    setTotalGames(games.length);
    setPhase('processing');
    startTimeRef.current = Date.now();

    // Concurrent worker pool
    let nextIndex = 0;
    const worker = async () => {
      while (nextIndex < games.length) {
        if (cancelledRef.current) {
          return;
        }
        const i = nextIndex++;
        await processGame(games[i], i);
      }
    };

    await Promise.all(
      Array.from({ length: Math.min(CONCURRENCY, games.length) }, () =>
        worker(),
      ),
    );

    // Mark remaining pending games as skipped on cancel
    if (cancelledRef.current) {
      for (let i = 0; i < games.length; i++) {
        if (resultsRef.current[i].status === 'pending') {
          completeGame(i, { status: 'skipped' as GameStatus });
        }
      }
      finalFlush();
      setElapsed(Math.floor((Date.now() - startTimeRef.current) / 1000));
      setPhase('cancelled');
    } else {
      finalFlush();
      setElapsed(Math.floor((Date.now() - startTimeRef.current) / 1000));
      setPhase('done');
    }

    processingRef.current = false;
  }, [completeGame, finalFlush, processGame]);

  useEffect(() => {
    startProcessing();
  }, [startProcessing]);

  const handleCancel = useCallback(() => {
    cancelledRef.current = true;
  }, []);

  const progressPercent =
    totalGames > 0 ? (completedGames / totalGames) * 100 : 0;
  const isDone = phase === 'done' || phase === 'cancelled';

  const getResult = useCallback(
    (index: number) => resultsRef.current[index],
    [],
  );

  const progressValue = useMemo(
    () => ({
      aggregates,
      completedGames,
      elapsed,
      handleCancel,
      isDone,
      phase,
      progressPercent,
      totalGames,
    }),
    [
      aggregates,
      completedGames,
      elapsed,
      handleCancel,
      isDone,
      phase,
      progressPercent,
      totalGames,
    ],
  );

  const resultsValue = useMemo(
    () => ({ getResult, resultCount: totalGames, version }),
    [getResult, totalGames, version],
  );

  return (
    <BulkUnlockProgressContext value={progressValue}>
      <BulkUnlockResultsContext value={resultsValue}>
        {children}
      </BulkUnlockResultsContext>
    </BulkUnlockProgressContext>
  );
};
