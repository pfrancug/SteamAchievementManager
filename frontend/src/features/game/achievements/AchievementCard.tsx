import type { AchievementCardProps } from '../types';
import type { MouseEvent } from 'react';

import { Box, HStack, Image, Menu, Portal, Tag, Text } from '@chakra-ui/react';
import { useAchievementActions } from '@providers/achievement-actions/useAchievementActions';
import { Tooltip } from '@shared/components/ui/tooltip';
import { useModifierKeys } from '@shared/hooks/useModifierKeys';
import {
  EyeOff,
  Lock,
  LockOpen,
  Pointer,
  PointerOff,
  Shield,
} from 'lucide-react';
import { useCallback } from 'react';

export const AchievementCard = ({
  achievement,
  selected,
  onSelect,
}: AchievementCardProps) => {
  const {
    onLock,
    onSelectAll,
    onSelectNone,
    onToggle,
    onUnlock,
    selectedCount,
  } = useAchievementActions();

  const isUnlocked = achievement.isUnlocked;
  const modifierActive = useModifierKeys();

  const handleClick = useCallback(
    (e: MouseEvent) => {
      onSelect(e.ctrlKey || e.metaKey, e.shiftKey);
    },
    [onSelect],
  );

  const handleDoubleClick = useCallback(
    (e: MouseEvent) => {
      e.stopPropagation();
      onToggle(achievement.id, achievement.isUnlocked, achievement.isProtected);
    },
    [achievement.id, achievement.isUnlocked, achievement.isProtected, onToggle],
  );

  return (
    <Menu.Root>
      <Menu.ContextTrigger asChild>
        <Box
          _hover={{ bg: selected ? 'cyan.400/12' : 'cyan.400/4' }}
          alignItems={'center'}
          as={'button'}
          bg={selected ? 'cyan.400/8' : 'transparent'}
          borderBottomWidth={'1px'}
          borderColor={selected ? 'cyan.400/40' : 'border.card'}
          cursor={modifierActive ? 'pointer' : 'default'}
          display={'grid'}
          gap={4}
          gridTemplateColumns={'64px 1fr auto'}
          h={'65px'}
          onClick={handleClick}
          onDoubleClick={handleDoubleClick}
          pr={4}
          tabIndex={-1}
          transition={'background 0.15s ease, border-color 0.15s ease'}
          w={'100%'}
        >
          <Box>
            <Image
              h={'64px'}
              objectFit={'cover'}
              w={'64px'}
              src={
                isUnlocked
                  ? achievement.iconUrl
                  : achievement.iconLockedUrl || achievement.iconUrl
              }
            />
          </Box>

          <Box>
            <Text
              fontSize={'sm'}
              fontWeight={'medium'}
              lineClamp={1}
              textAlign={'left'}
            >
              {achievement.name || achievement.id}
            </Text>

            {achievement.description && (
              <Tooltip label={achievement.description}>
                <Text
                  color={'fg.muted'}
                  fontSize={'xs'}
                  lineClamp={2}
                  textAlign={'left'}
                >
                  {achievement.description}
                </Text>
              </Tooltip>
            )}
          </Box>

          <HStack flexShrink={0} gap={'1'}>
            {achievement.isHidden && (
              <Tooltip label={'Hidden'}>
                <Tag.Root colorPalette={'white'} size={'sm'} variant={'subtle'}>
                  <Tag.StartElement m={0}>
                    <EyeOff />
                  </Tag.StartElement>
                </Tag.Root>
              </Tooltip>
            )}

            {!isUnlocked && (
              <Tooltip label={'Locked'}>
                <Tag.Root colorPalette={'white'} size={'sm'} variant={'subtle'}>
                  <Tag.StartElement m={0}>
                    <Lock />
                  </Tag.StartElement>
                </Tag.Root>
              </Tooltip>
            )}

            {achievement.isProtected && (
              <Tooltip
                label={'Protected — this achievement cannot be modified'}
              >
                <Tag.Root
                  colorPalette={'orange'}
                  size={'sm'}
                  variant={'subtle'}
                >
                  <Tag.StartElement m={0}>
                    <Shield />
                  </Tag.StartElement>
                </Tag.Root>
              </Tooltip>
            )}

            {isUnlocked && achievement.unlockTime && (
              <Tooltip
                label={`Unlocked on ${new Date(achievement.unlockTime).toLocaleString()}`}
              >
                <Tag.Root colorPalette={'teal'} size={'sm'} variant={'subtle'}>
                  <Tag.Label>
                    {new Date(achievement.unlockTime).toLocaleDateString()}
                  </Tag.Label>
                </Tag.Root>
              </Tooltip>
            )}
          </HStack>
        </Box>
      </Menu.ContextTrigger>

      <Portal>
        <Menu.Positioner>
          <Menu.Content>
            <Text color={'fg.muted'} fontSize={'xs'} px={'2'} py={'1'}>
              {`Achievement: ${achievement.name || achievement.id}`}
            </Text>

            <Menu.Item
              disabled={achievement.isProtected}
              value={'toggle-lock'}
              onSelect={() =>
                onToggle(
                  achievement.id,
                  achievement.isUnlocked,
                  achievement.isProtected,
                )
              }
            >
              {isUnlocked ? <Lock size={14} /> : <LockOpen size={14} />}
              <Menu.ItemText>{isUnlocked ? 'Lock' : 'Unlock'}</Menu.ItemText>
            </Menu.Item>

            <Menu.Separator />

            <Text color={'fg.muted'} fontSize={'xs'} px={'2'} py={'1'}>
              {`Selected: ${selectedCount}`}
            </Text>

            <Menu.Item
              disabled={selectedCount === 0}
              onSelect={onUnlock}
              value={'unlock-selected'}
            >
              <LockOpen size={14} />
              <Menu.ItemText>{'Unlock Selected'}</Menu.ItemText>
            </Menu.Item>

            <Menu.Item
              disabled={selectedCount === 0}
              onSelect={onLock}
              value={'lock-selected'}
            >
              <Lock size={14} />
              <Menu.ItemText>{'Lock Selected'}</Menu.ItemText>
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
