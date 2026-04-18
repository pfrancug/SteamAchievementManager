import type { StatDrawerProps } from '../types';

import {
  Alert,
  CloseButton,
  DataList,
  Drawer,
  For,
  Heading,
  HStack,
  Portal,
  Separator,
  Stack,
  Text,
} from '@chakra-ui/react';
import { useStatEditing } from '@providers/stat-editing/useStatEditing';
import { useMemo } from 'react';

import { StatValueEditor } from './StatValueEditor';

export const StatDrawer = ({ stats }: StatDrawerProps) => {
  const { activeStatId, onSave, optimisticValues, savingIds, setActiveStatId } =
    useStatEditing();

  const stat = useMemo(
    () => stats.find((s) => s.id === activeStatId),
    [stats, activeStatId],
  );

  const open = !!activeStatId && !!stat;
  const displayValue = stat ? (optimisticValues.get(stat.id) ?? stat.value) : 0;
  const isSaving = stat ? savingIds.has(stat.id) : false;
  const canEdit = stat ? !stat.isProtected && stat.type !== 'rate' : false;

  return (
    <Drawer.Root
      modal={false}
      open={open}
      placement={'end'}
      size={'md'}
      onOpenChange={({ open: isOpen }) => {
        if (!isOpen) {
          setActiveStatId(null);
        }
      }}
    >
      <Portal>
        <Drawer.Positioner pt={'36px'}>
          <Drawer.Content>
            <Drawer.Header>
              <Drawer.Title>{stat?.name || stat?.id}</Drawer.Title>
            </Drawer.Header>

            <Drawer.Body>
              {stat && (
                <Stack>
                  <HStack gap={6} h={'40px'}>
                    <Heading flexShrink={0} size={'md'}>
                      {canEdit ? 'Edit value' : 'Value'}
                    </Heading>

                    {canEdit ? (
                      <StatValueEditor
                        displayValue={displayValue}
                        isSaving={isSaving}
                        onSave={onSave}
                        stat={stat}
                      />
                    ) : (
                      <Text ml={'auto'} mr={20}>
                        {displayValue}
                      </Text>
                    )}
                  </HStack>

                  <Separator />

                  <Stack gap={2}>
                    <Heading flexShrink={0} size={'md'}>
                      {'Details'}
                    </Heading>

                    <For each={Object.entries(stat)}>
                      {([key, value]) => {
                        const parsedValue = String(value);

                        return (
                          <DataList.Root key={key} orientation={'horizontal'}>
                            <DataList.Item>
                              <DataList.ItemLabel>{key}</DataList.ItemLabel>
                              <DataList.ItemValue>
                                {parsedValue || (
                                  <Text color={'gray'} textStyle={'xs'}>
                                    {'—'}
                                  </Text>
                                )}
                              </DataList.ItemValue>
                            </DataList.Item>
                          </DataList.Root>
                        );
                      }}
                    </For>
                  </Stack>
                </Stack>
              )}
            </Drawer.Body>

            <Drawer.Footer>
              <Alert.Root
                colorPalette={canEdit ? 'red' : 'orange'}
                size={'sm'}
                status={canEdit ? 'warning' : 'error'}
                variant={'surface'}
              >
                <Alert.Indicator />
                <Alert.Content>
                  <Alert.Title>{canEdit ? 'Caution' : 'Protected'}</Alert.Title>

                  <Alert.Description>
                    {canEdit
                      ? 'Editing stats may cause unintended side effects in your game.'
                      : 'This stat is protected and cannot be edited.'}
                  </Alert.Description>
                </Alert.Content>
              </Alert.Root>
            </Drawer.Footer>

            <Drawer.CloseTrigger asChild>
              <CloseButton size={'sm'} />
            </Drawer.CloseTrigger>
          </Drawer.Content>
        </Drawer.Positioner>
      </Portal>
    </Drawer.Root>
  );
};
