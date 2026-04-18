interface ElectronAPI {
  getBackendPort: () => Promise<number>;
  openGameWindow: (
    appId: number,
    gameName: string,
    imageUrl?: string,
    purchaseTimestamp?: number,
    gameType?: string,
  ) => Promise<void>;
  onBackendReady: (callback: (port: number) => void) => () => void;
  openBulkUnlockWindow: (
    games: { appId: number; name: string; imageUrl?: string }[],
  ) => Promise<void>;
  getBulkGames: () => Promise<
    { appId: number; name: string; imageUrl?: string }[]
  >;
  startTempBackend: (appId: number) => Promise<number>;
  stopTempBackend: (port: number) => Promise<void>;
  minimizeWindow: () => Promise<void>;
  maximizeWindow: () => Promise<void>;
  closeWindow: () => Promise<void>;
  quitApp: () => Promise<void>;
  isMaximized: () => Promise<boolean>;
  onMaximizeChange: (callback: (maximized: boolean) => void) => () => void;
}

interface Window {
  electronAPI?: ElectronAPI;
}
