import { HStack, Progress, Separator, Stack, Tag } from '@chakra-ui/react';
import { useBulkUnlockProgress } from '@providers/bulk-unlock/useBulkUnlock';

import { formatTime } from '../utils';

export const BulkUnlockProgress = () => {
  const { aggregates, elapsed, isDone, phase, progressPercent, totalGames } =
    useBulkUnlockProgress();

  return (
    <Stack>
      <HStack>
        <Tag.Root colorPalette={'gray'} size={'sm'} variant={'surface'}>
          <Tag.Label>{`Progress: ${progressPercent.toFixed(1)}%`}</Tag.Label>
        </Tag.Root>

        {(phase === 'processing' || isDone) && (
          <Tag.Root colorPalette={'gray'} size={'sm'} variant={'surface'}>
            <Tag.Label>{`Time: ${formatTime(elapsed)}`}</Tag.Label>
          </Tag.Root>
        )}

        <Separator h={'100%'} orientation={'vertical'} />

        <Tag.Root colorPalette={'gray'} size={'sm'} variant={'surface'}>
          <Tag.Label>{`${totalGames} selected`}</Tag.Label>
        </Tag.Root>

        {aggregates.gamesDone > 0 && (
          <Tag.Root colorPalette={'green'} size={'sm'} variant={'surface'}>
            <Tag.Label>{`${aggregates.gamesDone} done`}</Tag.Label>
          </Tag.Root>
        )}

        {aggregates.gamesFailed > 0 && (
          <Tag.Root colorPalette={'red'} size={'sm'} variant={'surface'}>
            <Tag.Label>{`${aggregates.gamesFailed} failed`}</Tag.Label>
          </Tag.Root>
        )}

        {aggregates.gamesSkipped > 0 && (
          <Tag.Root colorPalette={'gray'} size={'sm'} variant={'surface'}>
            <Tag.Label>{`${aggregates.gamesSkipped} skipped`}</Tag.Label>
          </Tag.Root>
        )}
      </HStack>

      <Progress.Root
        striped
        animated={phase === 'processing'}
        colorPalette={'blue'}
        size={'sm'}
        value={progressPercent}
      >
        <Progress.Track>
          <Progress.Range />
        </Progress.Track>
      </Progress.Root>
    </Stack>
  );
};
