import { toaster } from '@shared/utils/toaster';
import { useCallback, useState } from 'react';

export const useRefreshAction = (refreshFn: () => Promise<unknown>) => {
  const [isRefreshing, setIsRefreshing] = useState(false);

  const handleRefresh = useCallback(async () => {
    setIsRefreshing(true);
    try {
      await refreshFn();
      toaster.create({
        title: 'Refreshed',
        description: 'Data reloaded',
        type: 'success',
      });
    } catch {
      toaster.create({
        title: 'Error',
        description: 'Failed to refresh',
        type: 'error',
      });
    } finally {
      setIsRefreshing(false);
    }
  }, [refreshFn]);

  return { handleRefresh, isRefreshing };
};
