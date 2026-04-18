import type { BulkAction, PendingToggle } from '../types';

import { AchievementActionsProvider } from '@providers/achievement-actions/AchievementActionsProvider';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { useCallback, useState } from 'react';

import { AchievementsContent } from './AchievementsContent';

interface AchievementsTabProps {
  loading: boolean;
}

export const AchievementsTab = ({ loading }: AchievementsTabProps) => {
  const { achievements } = useSteamHub();
  const [pendingAction, setPendingAction] = useState<BulkAction | null>(null);
  const [pendingToggle, setPendingToggle] = useState<PendingToggle | null>(
    null,
  );

  const handleToggle = useCallback(
    (id: string, isUnlocked: boolean, isProtected: boolean) => {
      if (isProtected) {
        return;
      }

      const achievement = achievements.find((a) => a.id === id);
      setPendingToggle({
        id,
        name: achievement?.name || id,
        unlock: !isUnlocked,
      });
    },
    [achievements],
  );

  return (
    <AchievementActionsProvider
      onLock={() => setPendingAction('lock')}
      onToggle={handleToggle}
      onUnlock={() => setPendingAction('unlock')}
    >
      <AchievementsContent
        loading={loading}
        pendingAction={pendingAction}
        pendingToggle={pendingToggle}
        setPendingAction={setPendingAction}
        setPendingToggle={setPendingToggle}
      />
    </AchievementActionsProvider>
  );
};
