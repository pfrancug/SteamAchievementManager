import type { StatEditingContextValue } from './types';

import { createContext } from 'react';

export const StatEditingContext = createContext<StatEditingContextValue | null>(
  null,
);
