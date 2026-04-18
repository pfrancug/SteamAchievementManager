import type { GamesActionBarProps } from '../types';

import {
  ActionBar,
  Button,
  IconButton,
  Kbd,
  Popover,
  Portal,
  Separator,
  Text,
} from '@chakra-ui/react';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { Info, LockOpen } from 'lucide-react';

export const GamesActionBar = ({
  disabled,
  onBulkUnlock,
  onSelectAll,
  onSelectNone,
  selectedCount,
}: GamesActionBarProps) => {
  const { games, gamesLoading, hubReady } = useSteamHub();
  const loading = !hubReady || gamesLoading || games.length === 0;

  return (
    <ActionBar.Root open={!loading}>
      <Portal>
        <ActionBar.Positioner>
          <ActionBar.Content>
            <ActionBar.SelectionTrigger>
              <Popover.Root>
                <Popover.Trigger asChild>
                  <IconButton size={'2xs'} variant={'plain'}>
                    <Info />
                  </IconButton>
                </Popover.Trigger>

                <Portal>
                  <Popover.Positioner>
                    <Popover.Content w={'380px'}>
                      <Popover.Arrow />

                      <Popover.Body>
                        <Popover.Title fontWeight={'medium'}>
                          {'Selection shortcuts'}
                        </Popover.Title>

                        <Text my={'4'}>
                          <Kbd>{'LMB'}</Kbd>
                          {' x2 to open game details.'}
                        </Text>

                        <Text my={'4'}>
                          <Kbd>{'Ctrl'}</Kbd>
                          {' + '}
                          <Kbd>{'LMB'}</Kbd>
                          {' to toggle selection.'}
                        </Text>

                        <Text my={'4'}>
                          <Kbd>{'Shift'}</Kbd>
                          {' + '}
                          <Kbd>{'LMB'}</Kbd>
                          {' to select a range from last selected.'}
                        </Text>

                        <Text my={'4'}>
                          <Kbd>{'RMB'}</Kbd>
                          {' to open more actions.'}
                        </Text>

                        <Separator />

                        <Text mb={'2'} mt={'4'}>
                          <Kbd>{'LMB'}</Kbd>
                          {' - Left Mouse Button'}
                        </Text>

                        <Text mt={'2'}>
                          <Kbd>{'RMB'}</Kbd>
                          {' - Right Mouse Button'}
                        </Text>
                      </Popover.Body>
                    </Popover.Content>
                  </Popover.Positioner>
                </Portal>
              </Popover.Root>
            </ActionBar.SelectionTrigger>

            <ActionBar.SelectionTrigger>
              {`${selectedCount} selected`}
            </ActionBar.SelectionTrigger>

            <ActionBar.Separator />

            <Button
              disabled={selectedCount === 0}
              onClick={onSelectNone}
              size={'sm'}
              variant={'outline'}
            >
              {'Select None'}
            </Button>

            <Button
              disabled={selectedCount === games.length || games.length === 0}
              onClick={onSelectAll}
              size={'sm'}
              variant={'outline'}
            >
              {'Select All'}
            </Button>

            <Button
              disabled={disabled || selectedCount === 0}
              onClick={onBulkUnlock}
              size={'sm'}
              variant={'outline'}
            >
              <LockOpen />
              {'Bulk Unlock'}
            </Button>
          </ActionBar.Content>
        </ActionBar.Positioner>
      </Portal>
    </ActionBar.Root>
  );
};
