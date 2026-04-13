import type { AchievementActionsContextValue } from './types';
import type { ReactNode } from 'react';

import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { useMultiSelect } from '@shared/hooks/useMultiSelect';
import { toaster } from '@shared/utils/toaster';
import { useCallback, useMemo, useRef } from 'react';

import { AchievementActionsContext } from './AchievementActionsContext';

interface AchievementActionsProviderProps {
  children: ReactNode;
  onLock: () => void;
  onToggle: (id: string, isUnlocked: boolean, isProtected: boolean) => void;
  onUnlock: () => void;
}

export const AchievementActionsProvider = ({
  children,
  onLock,
  onToggle,
  onUnlock,
}: AchievementActionsProviderProps) => {
  const {
    setAchievement,
    bulkSetAchievements,
    storeStats,
    refreshAchievements,
  } = useSteamHub();

  const multiSelect = useMultiSelect();
  const filteredIdsRef = useRef<string[]>([]);

  const setFilteredIds = useCallback((ids: string[]) => {
    filteredIdsRef.current = ids;
  }, []);

  const toggleSingle = useCallback(
    async (id: string, unlock: boolean) => {
      const ok = await setAchievement(id, unlock);
      if (!ok) {
        toaster.create({
          title: 'Error',
          description: `Failed to ${unlock ? 'unlock' : 'lock'} achievement`,
          type: 'error',
        });

        return;
      }

      await storeStats();

      const label = unlock ? 'Unlocked' : 'Locked';
      toaster.create({
        title: label,
        description: `${label} achievement`,
      });

      await refreshAchievements();
    },
    [setAchievement, storeStats, refreshAchievements],
  );

  const executeBulk = useCallback(
    async (ids: string[], unlock: boolean): Promise<number> => {
      const count = await bulkSetAchievements(ids, unlock);
      await storeStats();

      const label = unlock ? 'Unlocked' : 'Locked';
      toaster.create({
        title: `${label} Selected`,
        description: `${label} ${count} achievements`,
      });

      multiSelect.deselectAll();
      await refreshAchievements();

      return count;
    },
    [bulkSetAchievements, storeStats, refreshAchievements, multiSelect],
  );

  const value = useMemo<AchievementActionsContextValue>(
    () => ({
      executeBulk,
      multiSelect,
      onLock,
      onSelectAll: () => multiSelect.selectAll(filteredIdsRef.current),
      onSelectNone: () => multiSelect.deselectAll(),
      onToggle,
      onUnlock,
      selectedCount: multiSelect.selectedCount,
      setFilteredIds,
      toggleSingle,
    }),
    [
      executeBulk,
      multiSelect,
      onLock,
      onToggle,
      onUnlock,
      setFilteredIds,
      toggleSingle,
    ],
  );

  return (
    <AchievementActionsContext value={value}>
      {children}
    </AchievementActionsContext>
  );
};
