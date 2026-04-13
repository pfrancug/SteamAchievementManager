import type { GameInfo } from '@sam/shared';
import type { MultiSelectState } from '@shared/hooks/useMultiSelect';
import type { SortDir } from '@shared/types/sort';

export type SortField = 'name' | 'purchased' | 'appId';

export interface GameRowProps {
  game: GameInfo;
  isConnecting: boolean;
  isSelected: boolean;
  onBulkUnlock?: () => void;
  onBulkUnlockSelected: () => void;
  onOpen: () => void;
  onSelect: (ctrlKey: boolean, shiftKey: boolean) => void;
  onSelectAll: () => void;
  onSelectNone: () => void;
  selectedCount: number;
}

export interface GamesGridProps {
  multiSelect: MultiSelectState;
  onBulkUnlockSelected: () => void;
  onBulkUnlockSingle: (appId: number, name: string) => void;
  onOpen: (appId: number) => void;
  onRefresh: () => Promise<void>;
  onVisibleIdsChange: (ids: string[]) => void;
}

export interface GamesScrollListProps {
  allVisibleIds: string[];
  isConnecting: boolean;
  multiSelect: MultiSelectState;
  onBulkUnlockSelected: () => void;
  onBulkUnlockSingle: (appId: number, name: string) => void;
  onOpen: (appId: number) => void;
  sorted: GameInfo[];
}

export interface GamesActionBarProps {
  disabled: boolean;
  onBulkUnlock: () => void;
  onSelectAll: () => void;
  onSelectNone: () => void;
  selectedCount: number;
}

export interface GamesTableHeaderProps {
  onSort: (field: SortField) => void;
  sortDir: SortDir;
  sortField: SortField;
}
