import { Box } from '@chakra-ui/react';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { SteamErrorState } from '@shared/components/SteamErrorState';
import { useMultiSelect } from '@shared/hooks/useMultiSelect';
import { toaster } from '@shared/utils/toaster';
import { useCallback, useEffect, useRef } from 'react';

import { GamesActionBar } from './components/GamesActionBar';
import { GamesGrid } from './components/GamesGrid';

export const Games = () => {
  const { connectToGame, games, hubReady, loadGames, status } = useSteamHub();
  const multiSelect = useMultiSelect();
  const visibleIdsRef = useRef<string[]>([]);

  const handleVisibleIdsChange = useCallback((ids: string[]) => {
    visibleIdsRef.current = ids;
  }, []);

  const handleOpen = useCallback(
    async (appId: number) => {
      try {
        await connectToGame(appId);
      } catch (err) {
        toaster.create({
          title: 'Error',
          description: err instanceof Error ? err.message : 'Failed to connect',
          type: 'error',
        });
      }
    },
    [connectToGame],
  );

  const handleRefresh = useCallback(async () => {
    await loadGames();
  }, [loadGames]);

  useEffect(() => {
    if (hubReady && games.length === 0) {
      void loadGames();
    }
  }, [hubReady, games.length, loadGames]);

  const handleBulkUnlock = useCallback(() => {
    const selectedGames = games
      .filter((g) => multiSelect.isSelected(String(g.appId)))
      .map((g) => ({ appId: g.appId, imageUrl: g.imageUrl, name: g.name }));

    if (selectedGames.length === 0) {
      return;
    }

    if (window.electronAPI) {
      void window.electronAPI.openBulkUnlockWindow(selectedGames);
    }
    multiSelect.deselectAll();
  }, [games, multiSelect]);

  const handleBulkUnlockSingle = useCallback(
    (appId: number, name: string) => {
      if (window.electronAPI) {
        const game = games.find((g) => g.appId === appId);
        void window.electronAPI.openBulkUnlockWindow([
          { appId, imageUrl: game?.imageUrl, name },
        ]);
      }
    },
    [games],
  );

  if (status.error) {
    return <SteamErrorState />;
  }

  return (
    <Box
      display={'flex'}
      flex={1}
      flexDirection={'column'}
      minH={0}
      p={'4'}
      pb={'0'}
    >
      <GamesGrid
        multiSelect={multiSelect}
        onBulkUnlockSelected={handleBulkUnlock}
        onBulkUnlockSingle={handleBulkUnlockSingle}
        onOpen={handleOpen}
        onRefresh={handleRefresh}
        onVisibleIdsChange={handleVisibleIdsChange}
      />

      <GamesActionBar
        disabled={!window.electronAPI || !!status.error}
        onBulkUnlock={handleBulkUnlock}
        onSelectAll={() => multiSelect.selectAll(visibleIdsRef.current)}
        onSelectNone={() => multiSelect.deselectAll()}
        selectedCount={multiSelect.selectedCount}
      />
    </Box>
  );
};
