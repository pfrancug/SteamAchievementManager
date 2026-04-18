import type {
  BulkUnlockProgressContextValue,
  BulkUnlockResultsContextValue,
} from './types';

import { useContext } from 'react';

import {
  BulkUnlockProgressContext,
  BulkUnlockResultsContext,
} from './BulkUnlockContext';

export const useBulkUnlockProgress = (): BulkUnlockProgressContextValue => {
  const ctx = useContext(BulkUnlockProgressContext);

  if (!ctx) {
    throw new Error(
      'useBulkUnlockProgress must be used within BulkUnlockProvider',
    );
  }

  return ctx;
};

export const useBulkUnlockResults = (): BulkUnlockResultsContextValue => {
  const ctx = useContext(BulkUnlockResultsContext);

  if (!ctx) {
    throw new Error(
      'useBulkUnlockResults must be used within BulkUnlockProvider',
    );
  }

  return ctx;
};
