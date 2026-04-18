import type { BulkUnlockRowProps, GameStatus } from '../types';

import { Box, Heading, Image, Text } from '@chakra-ui/react';
import { memo } from 'react';

import { ROW_HEIGHT } from '../constants';
import { StatusIcon } from './StatusIcon';

const STATUS_MAP: Record<GameStatus, { color: string; label: string }> = {
  done: { color: 'green.400', label: 'Done' },
  failed: { color: 'red.400', label: 'Failed' },
  'in-progress': { color: 'blue.400', label: 'Processing...' },
  pending: { color: 'fg.muted', label: 'Waiting...' },
  skipped: { color: 'gray.400', label: 'Skipped' },
};

export const BulkUnlockRow = memo(({ result }: BulkUnlockRowProps) => {
  const isDone = result.status === 'done';
  const { color: statusColor, label: statusLabel } = STATUS_MAP[result.status];
  const statusText =
    result.status === 'failed' ? (result.error ?? statusLabel) : statusLabel;

  return (
    <Box
      alignItems={'center'}
      bg={result.status === 'in-progress' ? 'blue.400/5' : undefined}
      borderBottomWidth={'1px'}
      borderColor={'border.card'}
      display={'grid'}
      gap={4}
      gridTemplateColumns={'120px 1fr 60px 60px 60px 24px'}
      h={`${ROW_HEIGHT}px`}
      pr={4}
      w={'full'}
    >
      <Image h={'45px'} objectFit={'cover'} src={result.imageUrl} w={'120px'} />

      <Box>
        <Heading truncate size={'sm'}>
          {result.name}
        </Heading>

        <Text truncate color={statusColor} fontSize={'xs'}>
          {statusText}
        </Text>
      </Box>

      <Text
        color={isDone && result.unlocked > 0 ? 'teal.400' : 'fg.muted'}
        fontSize={'sm'}
        textAlign={'center'}
      >
        {isDone ? result.unlocked : '-'}
      </Text>

      <Text color={'fg.muted'} fontSize={'sm'} textAlign={'center'}>
        {isDone ? result.alreadyUnlocked : '-'}
      </Text>

      <Text
        color={isDone && result.protected > 0 ? 'orange.400' : 'fg.muted'}
        fontSize={'sm'}
        textAlign={'center'}
      >
        {isDone ? result.protected : '-'}
      </Text>

      <StatusIcon status={result.status} />
    </Box>
  );
});

BulkUnlockRow.displayName = 'BulkUnlockRow';
