import type { GameType } from '@sam/shared';

import { ChakraProvider } from '@chakra-ui/react';
import { BulkUnlockProvider } from '@providers/bulk-unlock/BulkUnlockProvider';
import { GameHubProvider } from '@providers/game-hub/GameHubProvider';
import { SteamHubProvider } from '@providers/steam-hub/SteamHubProvider';
import { Toaster } from '@shared/components/ui/toaster';
import { system } from '@theme/theme';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';

import { App } from './app/App';

const params = new URLSearchParams(window.location.search);
const mode = (params.get('mode') ?? 'picker') as
  | 'picker'
  | 'game'
  | 'bulk-unlock';

const appId = Number(params.get('appId') || 0);
const port = Number(params.get('port') || 0);
const gameName = params.get('gameName') ?? `App ${appId}`;
const imageUrl = params.get('imageUrl') ?? undefined;
const purchaseTimestampRaw = params.get('purchaseTimestamp');
const purchaseTimestamp = purchaseTimestampRaw
  ? Number(purchaseTimestampRaw)
  : undefined;
const gameType = (params.get('gameType') as GameType | null) ?? 'normal';

const renderApp = () => {
  switch (mode) {
    case 'game':
      if (appId > 0 && port > 0) {
        return (
          <GameHubProvider
            appId={appId}
            gameName={gameName}
            gameType={gameType}
            imageUrl={imageUrl}
            port={port}
            purchaseTimestamp={purchaseTimestamp}
          >
            <App gameName={gameName} mode={'game'} />
          </GameHubProvider>
        );
      }

      return (
        <SteamHubProvider>
          <App mode={'picker'} />
        </SteamHubProvider>
      );
    case 'bulk-unlock':
      return (
        <BulkUnlockProvider>
          <App mode={'bulk-unlock'} />
        </BulkUnlockProvider>
      );
    default:
      return (
        <SteamHubProvider>
          <App mode={'picker'} />
        </SteamHubProvider>
      );
  }
};

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ChakraProvider value={system}>
      <Toaster />
      {renderApp()}
    </ChakraProvider>
  </StrictMode>,
);
