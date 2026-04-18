import { BarSegment, useChart } from '@chakra-ui/charts';
import {
  HStack,
  Skeleton,
  SkeletonCircle,
  SkeletonText,
  Stack,
} from '@chakra-ui/react';
import { useBulkUnlockProgress } from '@providers/bulk-unlock/useBulkUnlock';
import { useMemo } from 'react';

export const BulkUnlockChart = () => {
  const { aggregates } = useBulkUnlockProgress();

  const chartData = useMemo(
    () =>
      [
        {
          name: 'Unlocked',
          value: aggregates.totalUnlocked,
          color: 'teal.400',
        },
        {
          name: 'Already Unlocked',
          value: aggregates.totalAlreadyUnlocked,
          color: 'gray.400',
        },
        {
          name: 'Protected',
          value: aggregates.totalProtected,
          color: 'orange.400',
        },
      ].filter((d) => d.value > 0),
    [aggregates],
  );

  const chart = useChart({ data: chartData });

  if (chartData.length === 0) {
    return (
      <Stack gap={4}>
        <Stack gap={1}>
          <SkeletonText h={'20px'} noOfLines={1} variant={'pulse'} w={'20px'} />
          <Skeleton h={'8px'} variant={'pulse'} />
        </Stack>
        <HStack>
          <SkeletonCircle size={'10px'} variant={'pulse'} />
          <SkeletonText
            h={'16px'}
            noOfLines={1}
            variant={'pulse'}
            w={'135px'}
          />
        </HStack>
      </Stack>
    );
  }

  return (
    <BarSegment.Root barSize={'2'} chart={chart}>
      <BarSegment.Content>
        <BarSegment.Value />
        <BarSegment.Bar />
      </BarSegment.Content>
      <BarSegment.Legend showPercent textStyle={'xs'} />
    </BarSegment.Root>
  );
};
