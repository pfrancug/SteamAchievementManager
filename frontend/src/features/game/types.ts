import type {
  AchievementInfo,
  GameInfo,
  StatInfo,
  SteamStatus,
} from '@sam/shared';
import type { SortDir } from '@shared/types/sort';

export interface GameHeaderProps {
  game: GameInfo | undefined;
  status: SteamStatus;
}

export interface ResetDialogProps {
  onClose: () => void;
  onReset: () => void;
  open: boolean;
}

export type BulkAction = 'unlock' | 'lock';

export interface BulkSummary {
  alreadyDone: number;
  isUnlock: boolean;
  protectedCount: number;
  willChange: number;
}

export type AchievementSortField = 'name' | 'unlockDate';

export type StatSortField = 'name' | 'value';

export interface PendingToggle {
  id: string;
  name: string;
  unlock: boolean;
}

export interface AchievementsContentProps {
  loading: boolean;
  pendingAction: BulkAction | null;
  pendingToggle: PendingToggle | null;
  setPendingAction: (action: BulkAction | null) => void;
  setPendingToggle: (toggle: PendingToggle | null) => void;
}

export interface AchievementsTableHeaderProps {
  onSort: (field: AchievementSortField) => void;
  sortDir: SortDir;
  sortField: AchievementSortField;
}

export interface StatsTableHeaderProps {
  onSort: (field: StatSortField) => void;
  sortDir: SortDir;
  sortField: StatSortField;
}

export interface AchievementCardProps {
  achievement: AchievementInfo;
  selected: boolean;
  onSelect: (ctrlKey: boolean, shiftKey: boolean) => void;
}

export interface BulkActionDialogProps {
  onClose: () => void;
  onConfirm: () => void;
  open: boolean;
  summary: BulkSummary | null;
}

export interface ToggleConfirmDialogProps {
  name: string;
  onClose: () => void;
  onConfirm: () => void;
  open: boolean;
  unlock: boolean;
}

export interface StatsScrollListProps {
  filtered: StatInfo[];
}

export interface StatRowProps {
  displayValue: number;
  isActive: boolean;
  isSaving: boolean;
  stat: StatInfo;
}

export interface StatDrawerProps {
  stats: StatInfo[];
}

export interface StatValueEditorProps {
  displayValue: number;
  isSaving: boolean;
  onSave: (id: string, type: 'int' | 'float' | 'rate', value: string) => void;
  stat: StatInfo;
}
