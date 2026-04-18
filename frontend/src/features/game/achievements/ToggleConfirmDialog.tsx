import type { ToggleConfirmDialogProps } from '../types';

import {
  Button,
  CloseButton,
  Dialog,
  Highlight,
  Portal,
  Text,
} from '@chakra-ui/react';
import { useState } from 'react';

export const ToggleConfirmDialog = ({
  name,
  onClose,
  onConfirm,
  open,
  unlock,
}: ToggleConfirmDialogProps) => {
  const [snapshot, setSnapshot] = useState({ name, unlock });

  if (open && (snapshot.name !== name || snapshot.unlock !== unlock)) {
    setSnapshot({ name, unlock });
  }

  return (
    <Dialog.Root
      open={open}
      placement={'center'}
      onOpenChange={(details) => {
        if (!details.open) {
          onClose();
        }
      }}
    >
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content>
            <Dialog.Header>
              <Dialog.Title>
                {`${snapshot.unlock ? 'Unlock' : 'Lock'} Achievement`}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color={'fg.muted'}>
                <Highlight
                  query={snapshot.name}
                  styles={{ bg: 'blue.subtle', color: 'blue.fg' }}
                >
                  {`Are you sure you want to ${snapshot.unlock ? 'unlock' : 'lock'} ${snapshot.name}?`}
                </Highlight>
              </Text>
            </Dialog.Body>

            <Dialog.Footer>
              <Dialog.ActionTrigger asChild>
                <Button
                  colorPalette={'white'}
                  onClick={onClose}
                  size={'xs'}
                  variant={'outline'}
                >
                  {'Cancel'}
                </Button>
              </Dialog.ActionTrigger>

              <Button
                autoFocus
                colorPalette={'gray'}
                onClick={onConfirm}
                size={'xs'}
                variant={'solid'}
              >
                {'Confirm'}
              </Button>
            </Dialog.Footer>

            <Dialog.CloseTrigger asChild>
              <CloseButton size={'sm'} />
            </Dialog.CloseTrigger>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
};
