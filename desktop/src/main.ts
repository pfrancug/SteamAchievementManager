import path from 'node:path';
import { app, BrowserWindow, ipcMain } from 'electron';
import { startBackend, killBackend, type BackendHandle } from './backend.js';

let mainWindow: BrowserWindow | null = null;
let browsingBackend: BackendHandle | null = null;

const iconPath = path.join(import.meta.dirname, '..', 'logo.png');

const gameWindows = new Map<
  number,
  { window: BrowserWindow; backend: BackendHandle }
>();

const isDev = !app.isPackaged;
const preloadPath = path.join(import.meta.dirname, 'preload.cjs');

const webPreferences = {
  preload: preloadPath,
  contextIsolation: true,
  nodeIntegration: false,
  sandbox: true,
} as const;

const createMainWindow = (): void => {
  mainWindow = new BrowserWindow({
    width: 1024,
    height: 728,
    minWidth: 1024,
    minHeight: 728,
    show: false,
    backgroundColor: '#0a0a0a',
    title: 'SAM-HC',
    icon: iconPath,
    frame: false,
    webPreferences,
  });

  mainWindow.on('closed', () => {
    mainWindow = null;
    // Close all game windows when main window closes
    for (const [, entry] of gameWindows) {
      entry.window.close();
    }
  });

  mainWindow.on('maximize', () => {
    mainWindow?.webContents.send('maximize-change', true);
  });
  mainWindow.on('unmaximize', () => {
    mainWindow?.webContents.send('maximize-change', false);
  });

  mainWindow.once('ready-to-show', () => {
    mainWindow?.show();
  });

  if (isDev) {
    mainWindow.loadURL('http://localhost:5174');
  } else {
    mainWindow.loadFile(
      path.join(import.meta.dirname, '..', 'renderer', 'index.html'),
    );
  }
};

const waitForHealth = async (
  port: number,
  retries = 30,
  interval = 500,
): Promise<void> => {
  for (let i = 0; i < retries; i++) {
    try {
      const response = await fetch(`http://localhost:${port}/health`);
      if (response.ok) return;
    } catch {
      // Backend not ready yet
    }
    await new Promise((r) => setTimeout(r, interval));
  }
  throw new Error('Backend health check failed after retries');
};

const bootstrap = async (): Promise<void> => {
  try {
    browsingBackend = await startBackend();
    await waitForHealth(browsingBackend.port);
    createMainWindow();
    mainWindow?.webContents.on('did-finish-load', () => {
      mainWindow?.webContents.send('backend-ready', browsingBackend!.port);
    });
  } catch {
    app.quit();
  }
};

// IPC: picker needs the browsing backend port
ipcMain.handle('get-backend-port', () => browsingBackend?.port ?? null);

// IPC: open a game in a new window
const pendingGameOpens = new Set<number>();
ipcMain.handle(
  'open-game-window',
  async (
    _event,
    appId: number,
    gameName: string,
    imageUrl?: string,
    purchaseTimestamp?: number,
    gameType?: string,
  ) => {
    // Focus existing window if already open
    const existing = gameWindows.get(appId);
    if (existing) {
      existing.window.focus();
      return;
    }

    // Prevent duplicate opens while window is being created
    if (pendingGameOpens.has(appId)) {
      return;
    }
    pendingGameOpens.add(appId);

    const backend = await startBackend(appId);

    const params = new URLSearchParams({
      mode: 'game',
      appId: String(appId),
      port: String(backend.port),
      gameName,
      ...(imageUrl ? { imageUrl } : {}),
      ...(purchaseTimestamp != null
        ? { purchaseTimestamp: String(purchaseTimestamp) }
        : {}),
      ...(gameType ? { gameType } : {}),
    });

    const gameWindow = new BrowserWindow({
      width: 1024,
      height: 728,
      minWidth: 1024,
      minHeight: 728,
      show: false,
      backgroundColor: '#0a0a0a',
      title: `SAM-HC — ${gameName}`,
      icon: iconPath,
      frame: false,
      webPreferences,
    });

    gameWindows.set(appId, { window: gameWindow, backend });
    pendingGameOpens.delete(appId);

    gameWindow.once('ready-to-show', () => {
      gameWindow.show();
    });

    gameWindow.on('maximize', () => {
      gameWindow.webContents.send('maximize-change', true);
    });
    gameWindow.on('unmaximize', () => {
      gameWindow.webContents.send('maximize-change', false);
    });

    gameWindow.on('closed', () => {
      gameWindows.delete(appId);
      killBackend(backend.process);
    });

    if (isDev) {
      gameWindow.loadURL(`http://localhost:5174?${params.toString()}`);
    } else {
      gameWindow.loadFile(
        path.join(import.meta.dirname, '..', 'renderer', 'index.html'),
        {
          search: params.toString(),
        },
      );
    }
  },
);

// Bulk unlock state
let pendingBulkGames: { appId: number; name: string; imageUrl?: string }[] = [];
const tempBackends = new Map<number, BackendHandle>();

// IPC: open bulk unlock window
ipcMain.handle(
  'open-bulk-unlock-window',
  (_event, games: { appId: number; name: string; imageUrl?: string }[]) => {
    pendingBulkGames = games;

    const bulkWindow = new BrowserWindow({
      width: 1024,
      height: 728,
      minWidth: 1024,
      minHeight: 728,
      show: false,
      backgroundColor: '#0a0a0a',
      title: 'SAM-HC — Bulk Unlock',
      icon: iconPath,
      frame: false,
      webPreferences,
    });

    bulkWindow.once('ready-to-show', () => {
      bulkWindow.show();
    });

    bulkWindow.on('maximize', () => {
      bulkWindow.webContents.send('maximize-change', true);
    });
    bulkWindow.on('unmaximize', () => {
      bulkWindow.webContents.send('maximize-change', false);
    });

    bulkWindow.on('closed', () => {
      // Kill any temp backends still running
      for (const [port, handle] of tempBackends) {
        killBackend(handle.process);
        tempBackends.delete(port);
      }
    });

    const params = new URLSearchParams({ mode: 'bulk-unlock' });

    if (isDev) {
      bulkWindow.loadURL(`http://localhost:5174?${params.toString()}`);
    } else {
      bulkWindow.loadFile(
        path.join(import.meta.dirname, '..', 'renderer', 'index.html'),
        {
          search: params.toString(),
        },
      );
    }
  },
);

// IPC: bulk unlock window fetches the game list
ipcMain.handle('get-bulk-games', () => pendingBulkGames);

// IPC: start a temporary backend for bulk processing
ipcMain.handle('start-temp-backend', async (_event, appId: number) => {
  const backend = await startBackend(appId);
  tempBackends.set(backend.port, backend);
  return backend.port;
});

// IPC: stop a temporary backend
ipcMain.handle('stop-temp-backend', async (_event, port: number) => {
  const backend = tempBackends.get(port);
  if (backend) {
    await killBackend(backend.process);
    tempBackends.delete(port);
  }
});

// IPC: window controls
ipcMain.handle('window-minimize', (event) => {
  BrowserWindow.fromWebContents(event.sender)?.minimize();
});

ipcMain.handle('window-maximize', (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  if (win?.isMaximized()) {
    win.unmaximize();
  } else {
    win?.maximize();
  }
});

ipcMain.handle('window-close', (event) => {
  BrowserWindow.fromWebContents(event.sender)?.close();
});

ipcMain.handle('quit-app', () => {
  app.quit();
});

ipcMain.handle('window-is-maximized', (event) => {
  return BrowserWindow.fromWebContents(event.sender)?.isMaximized() ?? false;
});

// App lifecycle
app.whenReady().then(bootstrap);

app.on('window-all-closed', () => {
  app.quit();
});

app.on('before-quit', async () => {
  // Kill all game backends
  const killPromises = [...gameWindows.values()].map((entry) =>
    killBackend(entry.backend.process),
  );
  // Kill browsing backend
  if (browsingBackend) {
    killPromises.push(killBackend(browsingBackend.process));
  }
  await Promise.all(killPromises);
});
