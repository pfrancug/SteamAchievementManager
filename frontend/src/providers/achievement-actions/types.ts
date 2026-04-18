import type { MultiSelectState } from '@shared/hooks/useMultiSelect';

export interface AchievementActionsContextValue {
  executeBulk: (ids: string[], unlock: boolean) => Promise<number>;
  multiSelect: MultiSelectState;
  onLock: () => void;
  onSelectAll: () => void;
  onSelectNone: () => void;
  onToggle: (id: string, isUnlocked: boolean, isProtected: boolean) => void;
  onUnlock: () => void;
  selectedCount: number;
  setFilteredIds: (ids: string[]) => void;
  toggleSingle: (id: string, unlock: boolean) => Promise<void>;
}
