import {
  type HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';

export type { HubConnection };
export { HubConnectionState };

export const createHubConnection = (port: number): HubConnection =>
  new HubConnectionBuilder()
    .withUrl(`http://localhost:${port}/hub`)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

export const getBackendPort = async (): Promise<number> => {
  if (window.electronAPI) {
    return window.electronAPI.getBackendPort();
  }
  // Fallback for standalone dev mode
  const envPort = import.meta.env.VITE_BACKEND_PORT;

  if (!envPort) {
    return 5000;
  }
  const parsed = parseInt(envPort as string, 10);

  return Number.isFinite(parsed) ? parsed : 5000;
};
