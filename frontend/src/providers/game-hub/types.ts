import type { GameType } from '@sam/shared';
import type { ReactNode } from 'react';

export interface GameHubProviderProps {
  children: ReactNode;
  port: number;
  appId: number;
  gameName: string;
  gameType: GameType;
  imageUrl?: string;
  purchaseTimestamp?: number;
}
