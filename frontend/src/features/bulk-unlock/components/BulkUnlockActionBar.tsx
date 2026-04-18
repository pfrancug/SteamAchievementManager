import { ActionBar, Button, Portal } from '@chakra-ui/react';
import { useBulkUnlockProgress } from '@providers/bulk-unlock/useBulkUnlock';
import { X } from 'lucide-react';

export const BulkUnlockActionBar = () => {
  const { handleCancel, isDone, phase } = useBulkUnlockProgress();

  return (
    <ActionBar.Root open>
      <Portal>
        <ActionBar.Positioner>
          <ActionBar.Content>
            {!isDone ? (
              <Button
                disabled={phase === 'loading'}
                onClick={handleCancel}
                size={'sm'}
                variant={'outline'}
              >
                <X />
                {'Cancel'}
              </Button>
            ) : (
              <Button
                onClick={() => window.close()}
                size={'sm'}
                variant={'outline'}
              >
                {'Close'}
              </Button>
            )}
          </ActionBar.Content>
        </ActionBar.Positioner>
      </Portal>
    </ActionBar.Root>
  );
};
