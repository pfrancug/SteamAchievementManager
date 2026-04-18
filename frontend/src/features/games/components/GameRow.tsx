import type { GameRowProps } from '../types';
import type { MouseEvent } from 'react';

import { Box, Image, Menu, Portal, Text } from '@chakra-ui/react';
import { useModifierKeys } from '@shared/hooks/useModifierKeys';
import { ExternalLink, LockOpen, Pointer, PointerOff } from 'lucide-react';
import { useCallback } from 'react';

export const GameRow = ({
  game,
  isConnecting,
  isSelected,
  onBulkUnlock,
  onBulkUnlockSelected,
  onOpen,
  onSelect,
  onSelectAll,
  onSelectNone,
  selectedCount,
}: GameRowProps) => {
  const modifierActive = useModifierKeys();

  const handleClick = useCallback(
    (e: MouseEvent) => {
      onSelect(e.ctrlKey || e.metaKey, e.shiftKey);
    },
    [onSelect],
  );

  const handleDoubleClick = useCallback(() => {
    onOpen();
  }, [onOpen]);

  return (
    <Menu.Root>
      <Menu.ContextTrigger asChild>
        <Box
          _hover={{ bg: isSelected ? 'cyan.400/12' : 'cyan.400/4' }}
          alignItems={'center'}
          aria-disabled={isConnecting || undefined}
          as={'button'}
          bg={isSelected ? 'cyan.400/8' : 'transparent'}
          borderBottomWidth={'1px'}
          borderColor={isSelected ? 'cyan.400/40' : 'border.card'}
          cursor={modifierActive ? 'pointer' : 'default'}
          display={'grid'}
          focusRing={'none'}
          gap={4}
          gridTemplateColumns={'160px 1fr 150px 150px'}
          h={'61px'}
          onClick={handleClick}
          onDoubleClick={handleDoubleClick}
          opacity={isConnecting ? 0.5 : 1}
          pointerEvents={isConnecting ? 'none' : undefined}
          pr={4}
          tabIndex={-1}
          transition={'background 0.15s ease, border-color 0.15s ease'}
          w={'100%'}
        >
          <Image
            h={'60px'}
            objectFit={'cover'}
            src={game.imageUrl}
            w={'160px'}
          />

          <Text
            fontSize={'sm'}
            fontWeight={'medium'}
            lineClamp={2}
            textAlign={'left'}
          >
            {game.name}
          </Text>

          <Text
            color={'fg.muted'}
            fontSize={'xs'}
            fontWeight={'normal'}
            textAlign={'right'}
          >
            {game.purchaseTimestamp
              ? new Date(game.purchaseTimestamp * 1000).toLocaleDateString()
              : '—'}
          </Text>

          <Text
            color={'fg.muted'}
            fontSize={'xs'}
            fontWeight={'normal'}
            textAlign={'right'}
          >
            {game.appId}
          </Text>
        </Box>
      </Menu.ContextTrigger>

      <Portal>
        <Menu.Positioner>
          <Menu.Content>
            <Text color={'fg.muted'} fontSize={'xs'} px={'2'} py={'1'}>
              {`Game: ${game.name}`}
            </Text>

            <Menu.Item onSelect={onOpen} value={'manage'}>
              <ExternalLink size={14} />
              <Menu.ItemText>{'Manage Achievements'}</Menu.ItemText>
            </Menu.Item>

            <Menu.Item
              disabled={!window.electronAPI}
              onSelect={onBulkUnlock}
              value={'unlock-all'}
            >
              <LockOpen size={14} />
              <Menu.ItemText>{'Unlock All Achievements'}</Menu.ItemText>
            </Menu.Item>

            <Menu.Separator />

            <Text color={'fg.muted'} fontSize={'xs'} px={'2'} py={'1'}>
              {`Selected: ${selectedCount}`}
            </Text>

            <Menu.Item
              disabled={selectedCount === 0 || !window.electronAPI}
              onSelect={onBulkUnlockSelected}
              value={'unlock-selected'}
            >
              <LockOpen size={14} />
              <Menu.ItemText>{'Unlock All for Selected'}</Menu.ItemText>
            </Menu.Item>

            <Menu.Separator />

            <Menu.Item onSelect={onSelectAll} value={'select-all'}>
              <Pointer size={14} />
              <Menu.ItemText>{'Select All'}</Menu.ItemText>
            </Menu.Item>

            <Menu.Item onSelect={onSelectNone} value={'select-none'}>
              <PointerOff size={14} />
              <Menu.ItemText>{'Select None'}</Menu.ItemText>
            </Menu.Item>
          </Menu.Content>
        </Menu.Positioner>
      </Portal>
    </Menu.Root>
  );
};
