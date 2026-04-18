import type { GameStatus } from './types';

export const statusColor = (status: GameStatus) => {
  switch (status) {
    case 'done':
      return 'teal';
    case 'failed':
      return 'red';
    case 'in-progress':
      return 'blue';
    case 'skipped':
      return 'gray';
    default:
      return 'gray';
  }
};

export const statusLabel = (status: GameStatus) => {
  switch (status) {
    case 'done':
      return 'Done';
    case 'failed':
      return 'Failed';
    case 'in-progress':
      return 'In Progress';
    case 'skipped':
      return 'Skipped';
    default:
      return 'Pending';
  }
};

export const formatTime = (seconds: number) => {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;

  return m > 0 ? `${m}m ${s}s` : `${s}s`;
};
