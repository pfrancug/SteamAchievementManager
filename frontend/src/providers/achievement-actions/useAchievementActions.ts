import type { AchievementActionsContextValue } from './types';

import { useContext } from 'react';

import { AchievementActionsContext } from './AchievementActionsContext';

export const useAchievementActions = (): AchievementActionsContextValue => {
  const ctx = useContext(AchievementActionsContext);

  if (!ctx) {
    throw new Error(
      'useAchievementActions must be used within AchievementActionsProvider',
    );
  }

  return ctx;
};
