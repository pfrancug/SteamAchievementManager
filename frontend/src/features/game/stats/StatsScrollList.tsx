import type { StatsScrollListProps } from '../types';

import { Box, ScrollArea } from '@chakra-ui/react';
import { useStatEditing } from '@providers/stat-editing/useStatEditing';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useRef } from 'react';

import { STATS_ROW_HEIGHT } from '../constants';
import { StatRow } from './StatRow';

export const StatsScrollList = ({ filtered }: StatsScrollListProps) => {
  const { activeStatId, optimisticValues, savingIds } = useStatEditing();
  const scrollRef = useRef<HTMLDivElement>(null);

  const virtualizer = useVirtualizer({
    count: filtered.length,
    estimateSize: () => STATS_ROW_HEIGHT,
    getScrollElement: () => scrollRef.current,
  });

  return (
    <ScrollArea.Root variant={'always'}>
      <ScrollArea.Viewport ref={scrollRef}>
        <ScrollArea.Content>
          <Box
            height={virtualizer.getTotalSize()}
            position={'relative'}
            width={'full'}
          >
            {virtualizer.getVirtualItems().map((virtualItem) => {
              const stat = filtered[virtualItem.index];

              return (
                <Box
                  // action bar + padding = 56px + 2rem
                  height={`calc(${virtualItem.size}px + 56px + 2rem)`}
                  key={virtualItem.key}
                  left={0}
                  position={'absolute'}
                  top={0}
                  transform={`translateY(${virtualItem.start}px)`}
                  width={'full'}
                >
                  <StatRow
                    displayValue={optimisticValues.get(stat.id) ?? stat.value}
                    isActive={activeStatId === stat.id}
                    isSaving={savingIds.has(stat.id)}
                    stat={stat}
                  />
                </Box>
              );
            })}
          </Box>
        </ScrollArea.Content>
      </ScrollArea.Viewport>

      <ScrollArea.Scrollbar bg={'transparent'} orientation={'vertical'} />
      <ScrollArea.Corner bg={'bg'} />
    </ScrollArea.Root>
  );
};
