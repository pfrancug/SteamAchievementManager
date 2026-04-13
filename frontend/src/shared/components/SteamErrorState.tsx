import {
  Button,
  EmptyState,
  HStack,
  Spinner,
  Text,
  VStack,
} from '@chakra-ui/react';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { toaster } from '@shared/utils/toaster';
import { TriangleAlert } from 'lucide-react';
import { useState } from 'react';

export const SteamErrorState = () => {
  const { retryConnect } = useSteamHub();
  const [isRetrying, setIsRetrying] = useState(false);

  const handleRetry = async () => {
    setIsRetrying(true);
    try {
      await retryConnect();
      toaster.create({
        title: 'Steam found',
        description: 'Loading data...',
        type: 'success',
      });
    } catch {
      toaster.create({
        title: 'Steam not found',
        description: 'Make sure Steam is running and try again.',
        type: 'error',
      });
    } finally {
      setIsRetrying(false);
    }
  };

  const handleClose = () => {
    if (window.electronAPI) {
      void window.electronAPI.quitApp();
    }
  };

  return (
    <EmptyState.Root flex={1}>
      <EmptyState.Content>
        <EmptyState.Indicator>
          <TriangleAlert color={'orange'} />
        </EmptyState.Indicator>

        <VStack textAlign={'center'}>
          <EmptyState.Title>{'Steam is not running'}</EmptyState.Title>
          <EmptyState.Description>
            {'Launch Steam and the app will reconnect automatically.'}
          </EmptyState.Description>
        </VStack>

        <VStack>
          <Spinner color={'gray'} size={'xs'} />
          <Text color={'gray'} textStyle={'xs'}>
            {'Waiting for Steam...'}
          </Text>
        </VStack>

        <HStack>
          <Button
            colorPalette={'white'}
            onClick={handleClose}
            size={'sm'}
            variant={'outline'}
          >
            {'Exit'}
          </Button>

          <Button
            colorPalette={'white'}
            loading={isRetrying}
            onClick={handleRetry}
            size={'sm'}
            variant={'solid'}
          >
            {'Retry now'}
          </Button>
        </HStack>
      </EmptyState.Content>
    </EmptyState.Root>
  );
};
