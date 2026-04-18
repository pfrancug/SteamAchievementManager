import type { AchievementsContentProps, BulkSummary } from '../types';

import { useAchievementActions } from '@providers/achievement-actions/useAchievementActions';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { useCallback, useMemo } from 'react';

import { AchievementGrid } from './AchievementGrid';
import { AchievementsActionBar } from './AchievementsActionBar';
import { BulkActionDialog } from './BulkActionDialog';
import { ToggleConfirmDialog } from './ToggleConfirmDialog';

export const AchievementsContent = ({
  loading,
  pendingAction,
  pendingToggle,
  setPendingAction,
  setPendingToggle,
}: AchievementsContentProps) => {
  const { achievements } = useSteamHub();
  const { executeBulk, multiSelect, toggleSingle } = useAchievementActions();

  const bulkSummary = useMemo((): BulkSummary | null => {
    if (!pendingAction) {
      return null;
    }

    const isUnlock = pendingAction === 'unlock';
    if (multiSelect.selectedCount === 0) {
      return null;
    }

    const pool = achievements.filter((a) => multiSelect.isSelected(a.id));

    const willChange = pool.filter(
      (a) => !a.isProtected && (isUnlock ? !a.isUnlocked : a.isUnlocked),
    ).length;
    const alreadyDone = pool.filter(
      (a) => !a.isProtected && (isUnlock ? a.isUnlocked : !a.isUnlocked),
    ).length;
    const protectedCount = pool.filter((a) => a.isProtected).length;

    return { alreadyDone, isUnlock, protectedCount, willChange };
  }, [pendingAction, achievements, multiSelect]);

  const executeBulkAction = useCallback(async () => {
    if (!pendingAction || !bulkSummary || bulkSummary.willChange === 0) {
      setPendingAction(null);

      return;
    }

    const isUnlock = bulkSummary.isUnlock;
    const pool = achievements.filter((a) => multiSelect.isSelected(a.id));
    const targets = pool.filter(
      (a) => !a.isProtected && (isUnlock ? !a.isUnlocked : a.isUnlocked),
    );

    setPendingAction(null);
    await executeBulk(
      targets.map((a) => a.id),
      isUnlock,
    );
  }, [
    pendingAction,
    bulkSummary,
    achievements,
    multiSelect,
    executeBulk,
    setPendingAction,
  ]);

  const confirmToggle = useCallback(async () => {
    if (!pendingToggle) {
      return;
    }

    const { id, unlock } = pendingToggle;
    setPendingToggle(null);
    await toggleSingle(id, unlock);
  }, [pendingToggle, toggleSingle, setPendingToggle]);

  return (
    <>
      <AchievementGrid loading={loading} />

      <AchievementsActionBar disabled={loading} open={!loading} />

      <BulkActionDialog
        onClose={() => setPendingAction(null)}
        onConfirm={executeBulkAction}
        open={!!pendingAction}
        summary={bulkSummary}
      />

      <ToggleConfirmDialog
        name={pendingToggle?.name ?? ''}
        onClose={() => setPendingToggle(null)}
        onConfirm={confirmToggle}
        open={!!pendingToggle}
        unlock={pendingToggle?.unlock ?? true}
      />
    </>
  );
};
