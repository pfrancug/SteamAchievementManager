import type { SteamHubContextValue } from './types';

import { useContext } from 'react';

import { SteamHubContext } from './SteamHubContext';

export const useSteamHub = (): SteamHubContextValue => {
  const ctx = useContext(SteamHubContext);
  if (!ctx) {
    throw new Error('useSteamHub must be used within SteamHubProvider');
  }

  return ctx;
};
