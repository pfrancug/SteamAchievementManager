import type { SteamHubContextValue } from './types';

import { createContext } from 'react';

export const SteamHubContext = createContext<SteamHubContextValue | null>(null);
