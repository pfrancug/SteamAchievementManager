import type { StatEditingContextValue } from './types';

import { useContext } from 'react';

import { StatEditingContext } from './StatEditingContext';

export const useStatEditing = (): StatEditingContextValue => {
  const ctx = useContext(StatEditingContext);
  if (!ctx) {
    throw new Error('useStatEditing must be used within StatEditingProvider');
  }

  return ctx;
};
