import type {
  ChartAggregates,
  GameResult,
  Phase,
} from '@features/bulk-unlock/types';

export interface BulkUnlockProgressContextValue {
  aggregates: ChartAggregates;
  completedGames: number;
  elapsed: number;
  handleCancel: () => void;
  isDone: boolean;
  phase: Phase;
  progressPercent: number;
  totalGames: number;
}

export interface BulkUnlockResultsContextValue {
  getResult: (index: number) => GameResult;
  resultCount: number;
  version: number;
}
