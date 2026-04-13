import type { ChildProcess } from 'node:child_process';
import { spawn } from 'node:child_process';
import path from 'node:path';
import { app } from 'electron';

export interface BackendHandle {
  process: ChildProcess;
  port: number;
}

const getBackendPath = (): string => {
  if (app.isPackaged) {
    return path.join(process.resourcesPath, 'backend', 'SAM.Backend.exe');
  }
  return path.join(
    app.getAppPath(),
    '..',
    'backend',
    'bin',
    'Debug',
    'net10.0-windows',
    'win-x64',
    'SAM.Backend.exe',
  );
};

export const startBackend = (appId?: number): Promise<BackendHandle> =>
  new Promise((resolve, reject) => {
    const exePath = getBackendPath();
    const args: string[] = [];
    if (appId !== undefined && appId > 0) {
      args.push(`--appid=${appId}`);
    }
    const child = spawn(exePath, args, {
      stdio: ['ignore', 'pipe', 'pipe'],
      windowsHide: true,
    });

    let resolved = false;

    child.stdout?.on('data', (data: Buffer) => {
      const text = data.toString();
      const match = /PORT:(\d+)/.exec(text);
      if (match && !resolved) {
        resolved = true;
        resolve({ process: child, port: parseInt(match[1], 10) });
      }
    });

    child.stderr?.on('data', (data: Buffer) => {
      console.error('[backend]', data.toString().trimEnd());
    });

    child.on('error', (err) => {
      if (!resolved) {
        resolved = true;
        reject(new Error(`Failed to start backend: ${err.message}`));
      }
    });

    child.on('exit', (code) => {
      if (!resolved) {
        resolved = true;
        reject(
          new Error(`Backend exited with code ${code} before reporting port`),
        );
      }
    });

    setTimeout(() => {
      if (!resolved) {
        resolved = true;
        killBackend(child);
        reject(new Error('Backend startup timed out (15s)'));
      }
    }, 15_000);
  });

export const killBackend = (proc: ChildProcess): Promise<void> =>
  new Promise((resolve) => {
    if (proc.killed || proc.exitCode !== null) {
      resolve();
      return;
    }

    const forceKillTimeout = setTimeout(() => {
      proc.kill('SIGKILL');
      resolve();
    }, 5_000);

    proc.on('exit', () => {
      clearTimeout(forceKillTimeout);
      resolve();
    });

    proc.kill('SIGTERM');
  });
