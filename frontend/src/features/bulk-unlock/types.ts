export interface ChartAggregates {
  gamesDone: number;
  gamesFailed: number;
  gamesSkipped: number;
  totalAlreadyUnlocked: number;
  totalProtected: number;
  totalUnlocked: number;
}

export type GameStatus =
  | 'pending'
  | 'in-progress'
  | 'done'
  | 'skipped'
  | 'failed';

export interface GameResult {
  alreadyUnlocked: number;
  appId: number;
  error?: string;
  imageUrl?: string;
  name: string;
  protected: number;
  status: GameStatus;
  total: number;
  unlocked: number;
}

export type Phase = 'loading' | 'processing' | 'done' | 'cancelled';

export interface StatusIconProps {
  status: GameStatus;
}

export interface BulkUnlockRowProps {
  result: GameResult;
}

export type BulkUnlockSortField =
  | 'name'
  | 'status'
  | 'unlocked'
  | 'alreadyUnlocked'
  | 'protected';

export interface BulkUnlockTableHeaderProps {
  onSort: (field: BulkUnlockSortField) => void;
  sortDir: 'asc' | 'desc';
  sortField: BulkUnlockSortField;
}
