import type { ResetDialogProps } from '../types';

import {
  Button,
  Dialog,
  Highlight,
  HStack,
  Portal,
  Text,
} from '@chakra-ui/react';

export const ResetDialog = ({ onClose, onReset, open }: ResetDialogProps) => (
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
            <Dialog.Title>{'Reset'}</Dialog.Title>
          </Dialog.Header>

          <Dialog.Body>
            <Text color={'fg.muted'}>
              <Highlight
                query={'reset all stats'}
                styles={{ bg: 'red.subtle', color: 'red.fg' }}
              >
                {`Are you sure you want to reset all stats for this game to their default values?`}
              </Highlight>
            </Text>
          </Dialog.Body>

          <Dialog.Footer>
            <HStack gap={'2'} justify={'flex-end'}>
              <Button
                colorPalette={'gray'}
                onClick={onClose}
                size={'xs'}
                variant={'outline'}
              >
                {'Cancel'}
              </Button>

              <Button
                colorPalette={'red'}
                gap={2}
                onClick={onReset}
                size={'xs'}
                variant={'solid'}
              >
                {'Confirm'}
              </Button>
            </HStack>
          </Dialog.Footer>
        </Dialog.Content>
      </Dialog.Positioner>
    </Portal>
  </Dialog.Root>
);
