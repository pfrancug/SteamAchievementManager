import type { StatEditingContextValue } from './types';
import type { ReactNode } from 'react';

import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { toaster } from '@shared/utils/toaster';
import { useCallback, useMemo, useState } from 'react';

import { StatEditingContext } from './StatEditingContext';

export const StatEditingProvider = ({ children }: { children: ReactNode }) => {
  const { setStat, storeStats, refreshStats } = useSteamHub();
  const [savingIds, setSavingIds] = useState<Set<string>>(new Set());
  const [activeStatId, setActiveStatId] = useState<string | null>(null);
  const [optimisticValues, setOptimisticValues] = useState<Map<string, number>>(
    new Map(),
  );

  const onSave = useCallback(
    async (id: string, type: 'int' | 'float' | 'rate', raw: string) => {
      const value = type === 'int' ? parseInt(raw, 10) : parseFloat(raw);
      if (isNaN(value)) {
        toaster.create({ title: 'Invalid value', type: 'error' });

        return;
      }

      setSavingIds((prev) => new Set(prev).add(id));
      setOptimisticValues((prev) => new Map(prev).set(id, value));

      const ok = await setStat(id, value, type);

      if (!ok) {
        toaster.create({ title: 'Failed to set stat', type: 'error' });
        setSavingIds((prev) => {
          const next = new Set(prev);
          next.delete(id);

          return next;
        });
        setOptimisticValues((prev) => {
          const next = new Map(prev);
          next.delete(id);

          return next;
        });

        return;
      }

      const stored = await storeStats();

      if (!stored) {
        toaster.create({ title: 'Failed to save to Steam', type: 'error' });
        setOptimisticValues((prev) => {
          const next = new Map(prev);
          next.delete(id);

          return next;
        });
      } else {
        toaster.create({ title: `Saved ${id} = ${value}`, type: 'success' });
      }

      setSavingIds((prev) => {
        const next = new Set(prev);
        next.delete(id);

        return next;
      });

      await refreshStats();

      setOptimisticValues((prev) => {
        const next = new Map(prev);
        next.delete(id);

        return next;
      });
    },
    [setStat, storeStats, refreshStats],
  );

  const ctx = useMemo<StatEditingContextValue>(
    () => ({
      activeStatId,
      onSave,
      optimisticValues,
      savingIds,
      setActiveStatId,
    }),
    [activeStatId, onSave, optimisticValues, savingIds],
  );

  return <StatEditingContext value={ctx}>{children}</StatEditingContext>;
};
