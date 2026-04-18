import type {
  BulkUnlockProgressContextValue,
  BulkUnlockResultsContextValue,
} from './types';

import { createContext } from 'react';

export const BulkUnlockProgressContext =
  createContext<BulkUnlockProgressContextValue | null>(null);

export const BulkUnlockResultsContext =
  createContext<BulkUnlockResultsContextValue | null>(null);
