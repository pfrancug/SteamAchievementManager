export const GAME_TYPE_LABELS: Record<string, string> = {
  normal: 'Game',
  demo: 'Demo',
  mod: 'Mod',
  junk: 'Junk',
};

export const ACHIEVEMENT_FILTER_OPTIONS: { label: string; value: string }[] = [
  { label: 'Locked', value: 'locked' },
  { label: 'Unlocked', value: 'unlocked' },
  { label: 'Protected', value: 'protected' },
  { label: 'Hidden', value: 'hidden' },
];

export const ACHIEVEMENT_ROW_HEIGHT = 65;

export const STATS_FILTER_OPTIONS: { label: string; value: string }[] = [
  { label: 'Unprotected', value: 'unprotected' },
  { label: 'Protected', value: 'protected' },
];

export const STATS_ROW_HEIGHT = 41;
