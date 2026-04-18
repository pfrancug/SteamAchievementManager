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
import { useAchievementActions } from '@providers/achievement-actions/useAchievementActions';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { Info, Lock, LockOpen } from 'lucide-react';

interface AchievementsActionBarProps {
  disabled?: boolean;
  open: boolean;
}

export const AchievementsActionBar = ({
  disabled,
  open,
}: AchievementsActionBarProps) => {
  const { achievements } = useSteamHub();
  const { onLock, onSelectAll, onSelectNone, onUnlock, selectedCount } =
    useAchievementActions();

  return (
    <ActionBar.Root open={open}>
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
                          {' x2 to lock/unlock achievement.'}
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
              disabled={disabled || selectedCount === 0}
              onClick={onSelectNone}
              size={'sm'}
              variant={'outline'}
            >
              {'Select None'}
            </Button>

            <Button
              disabled={disabled || selectedCount === achievements.length}
              onClick={onSelectAll}
              size={'sm'}
              variant={'outline'}
            >
              {'Select All'}
            </Button>

            <Button
              disabled={disabled || selectedCount === 0}
              onClick={onLock}
              size={'sm'}
              variant={'outline'}
            >
              <Lock />
              {'Lock Selected'}
            </Button>

            <Button
              disabled={disabled || selectedCount === 0}
              onClick={onUnlock}
              size={'sm'}
              variant={'outline'}
            >
              <LockOpen />
              {'Unlock Selected'}
            </Button>
          </ActionBar.Content>
        </ActionBar.Positioner>
      </Portal>
    </ActionBar.Root>
  );
};
