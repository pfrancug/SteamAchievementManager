import { useCallback, useMemo, useState } from 'react';

export interface MultiSelectState {
  selectedIds: Set<string>;
  lastSelectedIndex: number | null;
  isSelected: (id: string) => boolean;
  toggleSelect: (
    id: string,
    index: number,
    ctrlKey: boolean,
    shiftKey: boolean,
    allIds: string[],
  ) => void;
  selectAll: (ids: string[]) => void;
  deselectAll: () => void;
  selectedCount: number;
}

export const useMultiSelect = (): MultiSelectState => {
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
  const [lastSelectedIndex, setLastSelectedIndex] = useState<number | null>(
    null,
  );

  const isSelected = useCallback(
    (id: string) => selectedIds.has(id),
    [selectedIds],
  );

  const toggleSelect = useCallback(
    (
      id: string,
      index: number,
      ctrlKey: boolean,
      shiftKey: boolean,
      allIds: string[],
    ) => {
      if (shiftKey && lastSelectedIndex !== null) {
        // Range: add items between anchor and clicked, don't move anchor
        setSelectedIds((prev) => {
          const next = new Set(prev);
          const start = Math.min(lastSelectedIndex, index);
          const end = Math.max(lastSelectedIndex, index);
          for (let i = start; i <= end; i++) {
            next.add(allIds[i]);
          }

          return next;
        });
      } else if (ctrlKey) {
        // Toggle individual item, update anchor
        setSelectedIds((prev) => {
          const next = new Set(prev);
          if (next.has(id)) {
            next.delete(id);
          } else {
            next.add(id);
          }

          return next;
        });
        setLastSelectedIndex(index);
      } else if (shiftKey) {
        // Shift with no anchor: select only clicked, set anchor
        setSelectedIds(new Set([id]));
        setLastSelectedIndex(index);
      } else {
        // Plain click: select only clicked item and update anchor
        setSelectedIds(new Set([id]));
        setLastSelectedIndex(index);
      }
    },
    [lastSelectedIndex],
  );

  const selectAll = useCallback((ids: string[]) => {
    setSelectedIds(new Set(ids));
  }, []);

  const deselectAll = useCallback(() => {
    setSelectedIds(new Set());
    setLastSelectedIndex(null);
  }, []);

  const selectedCount = useMemo(() => selectedIds.size, [selectedIds]);

  return {
    selectedIds,
    lastSelectedIndex,
    isSelected,
    toggleSelect,
    selectAll,
    deselectAll,
    selectedCount,
  };
};
