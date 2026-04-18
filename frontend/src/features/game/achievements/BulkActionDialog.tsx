import type { BulkActionDialogProps, BulkSummary } from '../types';

import {
  Button,
  CloseButton,
  Dialog,
  Highlight,
  HStack,
  Icon,
  List,
  Portal,
  Text,
} from '@chakra-ui/react';
import { Check, Lock, LockOpen, Shield } from 'lucide-react';
import { useState } from 'react';

const EMPTY_SUMMARY: BulkSummary = {
  alreadyDone: 0,
  isUnlock: true,
  protectedCount: 0,
  willChange: 0,
};

export const BulkActionDialog = ({
  onClose,
  onConfirm,
  open,
  summary,
}: BulkActionDialogProps) => {
  const [snapshot, setSnapshot] = useState<BulkSummary>(EMPTY_SUMMARY);

  if (open && summary && summary !== snapshot) {
    setSnapshot(summary);
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
                {`${snapshot.isUnlock ? 'Unlock' : 'Lock'} Achievements`}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color={'fg.muted'} mb={'3'}>
                {`Are you sure you want to ${snapshot.isUnlock ? 'unlock' : 'lock'} the selected achievements?`}
              </Text>

              <List.Root align={'center'} gap={'2'} variant={'plain'}>
                <List.Item color={'fg.muted'}>
                  <List.Indicator
                    asChild
                    color={snapshot.isUnlock ? 'teal.400' : 'white.400'}
                  >
                    <Icon size={'sm'}>
                      {snapshot.isUnlock ? <LockOpen /> : <Lock />}
                    </Icon>
                  </List.Indicator>

                  <HStack>
                    {`Will be ${snapshot.isUnlock ? 'unlocked' : 'locked'}:`}

                    <Highlight
                      query={snapshot.willChange.toString()}
                      styles={{ color: 'white', fontWeight: 'medium' }}
                    >
                      {snapshot.willChange.toString()}
                    </Highlight>
                  </HStack>
                </List.Item>

                {snapshot.alreadyDone > 0 && (
                  <List.Item color={'fg.muted'}>
                    <List.Indicator asChild color={'gray'}>
                      <Icon size={'sm'}>
                        <Check />
                      </Icon>
                    </List.Indicator>

                    <HStack>
                      {`Already ${snapshot.isUnlock ? 'unlocked' : 'locked'}:`}

                      <Highlight
                        query={snapshot.alreadyDone.toString()}
                        styles={{ color: 'white', fontWeight: 'medium' }}
                      >
                        {snapshot.alreadyDone.toString()}
                      </Highlight>
                    </HStack>
                  </List.Item>
                )}

                {snapshot.protectedCount > 0 && (
                  <List.Item color={'fg.muted'}>
                    <List.Indicator asChild color={'orange.400'}>
                      <Icon size={'sm'}>
                        <Shield />
                      </Icon>
                    </List.Indicator>

                    <HStack>
                      {'Protected (skipped):'}

                      <Highlight
                        query={snapshot.protectedCount.toString()}
                        styles={{ color: 'white', fontWeight: 'medium' }}
                      >
                        {snapshot.protectedCount.toString()}
                      </Highlight>
                    </HStack>
                  </List.Item>
                )}
              </List.Root>
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
                colorPalette={'white'}
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
