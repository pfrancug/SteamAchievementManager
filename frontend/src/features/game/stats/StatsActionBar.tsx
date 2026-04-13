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
import { Info, RotateCcw } from 'lucide-react';

interface StatsActionBarProps {
  disabled?: boolean;
  onReset: () => void;
  open: boolean;
}

export const StatsActionBar = ({
  disabled,
  onReset,
  open,
}: StatsActionBarProps) => (
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
                        {' to open details.'}
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

          <Button
            disabled={disabled}
            onClick={onReset}
            size={'sm'}
            variant={'outline'}
          >
            <RotateCcw />

            {'Reset Stats'}
          </Button>
        </ActionBar.Content>
      </ActionBar.Positioner>
    </Portal>
  </ActionBar.Root>
);
