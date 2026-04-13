const { contextBridge, ipcRenderer } =
  require('electron') as typeof import('electron');

const electronAPI = {
  getBackendPort: (): Promise<number> => ipcRenderer.invoke('get-backend-port'),
  openGameWindow: (
    appId: number,
    gameName: string,
    imageUrl?: string,
    purchaseTimestamp?: number,
    gameType?: string,
  ): Promise<void> =>
    ipcRenderer.invoke(
      'open-game-window',
      appId,
      gameName,
      imageUrl,
      purchaseTimestamp,
      gameType,
    ),
  onBackendReady: (callback: (port: number) => void) => {
    const handler = (_event: Electron.IpcRendererEvent, port: number) =>
      callback(port);
    ipcRenderer.on('backend-ready', handler);
    return () => ipcRenderer.removeListener('backend-ready', handler);
  },
  openBulkUnlockWindow: (
    games: { appId: number; name: string; imageUrl?: string }[],
  ): Promise<void> => ipcRenderer.invoke('open-bulk-unlock-window', games),
  getBulkGames: (): Promise<
    { appId: number; name: string; imageUrl?: string }[]
  > => ipcRenderer.invoke('get-bulk-games'),
  startTempBackend: (appId: number): Promise<number> =>
    ipcRenderer.invoke('start-temp-backend', appId),
  stopTempBackend: (port: number): Promise<void> =>
    ipcRenderer.invoke('stop-temp-backend', port),
  minimizeWindow: (): Promise<void> => ipcRenderer.invoke('window-minimize'),
  maximizeWindow: (): Promise<void> => ipcRenderer.invoke('window-maximize'),
  closeWindow: (): Promise<void> => ipcRenderer.invoke('window-close'),
  quitApp: (): Promise<void> => ipcRenderer.invoke('quit-app'),
  isMaximized: (): Promise<boolean> =>
    ipcRenderer.invoke('window-is-maximized'),
  onMaximizeChange: (callback: (maximized: boolean) => void) => {
    const handler = (_event: Electron.IpcRendererEvent, maximized: boolean) =>
      callback(maximized);
    ipcRenderer.on('maximize-change', handler);
    return () => ipcRenderer.removeListener('maximize-change', handler);
  },
};

contextBridge.exposeInMainWorld('electronAPI', electronAPI);
