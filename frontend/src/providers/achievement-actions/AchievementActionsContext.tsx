import type { AchievementActionsContextValue } from './types';

import { createContext } from 'react';

export const AchievementActionsContext =
  createContext<AchievementActionsContextValue | null>(null);
