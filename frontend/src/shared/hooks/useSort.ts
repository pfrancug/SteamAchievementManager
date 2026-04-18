import type { SortDir } from '@shared/types/sort';

import { useCallback, useState } from 'react';

export const useSort = <T extends string>(
  defaultField: T,
  defaultDir: SortDir = 'asc',
) => {
  const [sortField, setSortField] = useState<T>(defaultField);
  const [sortDir, setSortDir] = useState<SortDir>(defaultDir);

  const toggleSort = useCallback(
    (field: T) => {
      if (sortField === field) {
        setSortDir((d) => (d === 'asc' ? 'desc' : 'asc'));
      } else {
        setSortField(field);
        setSortDir(field === defaultField ? defaultDir : 'desc');
      }
    },
    [sortField, defaultField, defaultDir],
  );

  return { sortDir, sortField, toggleSort };
};
