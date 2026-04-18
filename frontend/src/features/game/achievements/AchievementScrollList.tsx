import type { AchievementInfo } from '@sam/shared';

import { Box, ScrollArea } from '@chakra-ui/react';
import { useAchievementActions } from '@providers/achievement-actions/useAchievementActions';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useRef } from 'react';

import { ACHIEVEMENT_ROW_HEIGHT } from '../constants';
import { AchievementCard } from './AchievementCard';

interface AchievementScrollListProps {
  filtered: AchievementInfo[];
  filteredIds: string[];
}

export const AchievementScrollList = ({
  filtered,
  filteredIds,
}: AchievementScrollListProps) => {
  const { multiSelect } = useAchievementActions();
  const scrollRef = useRef<HTMLDivElement>(null);

  const virtualizer = useVirtualizer({
    count: filtered.length,
    estimateSize: () => ACHIEVEMENT_ROW_HEIGHT,
    getScrollElement: () => scrollRef.current,
    paddingEnd: 88,
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
            {virtualizer.getVirtualItems().map((virtualRow) => {
              const achievement = filtered[virtualRow.index];

              return (
                <Box
                  height={virtualRow.size}
                  key={achievement.id}
                  left={0}
                  position={'absolute'}
                  top={0}
                  transform={`translateY(${virtualRow.start}px)`}
                  width={'full'}
                >
                  <AchievementCard
                    achievement={achievement}
                    selected={multiSelect.isSelected(achievement.id)}
                    onSelect={(ctrlKey, shiftKey) => {
                      multiSelect.toggleSelect(
                        achievement.id,
                        virtualRow.index,
                        ctrlKey,
                        shiftKey,
                        filteredIds,
                      );
                    }}
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
