import type { GamesScrollListProps } from '../types';

import { Box, ScrollArea } from '@chakra-ui/react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useRef } from 'react';

import { ROW_HEIGHT } from '../constants';
import { GameRow } from './GameRow';

export const GamesScrollList = ({
  allVisibleIds,
  isConnecting,
  multiSelect,
  onBulkUnlockSelected,
  onBulkUnlockSingle,
  onOpen,
  sorted,
}: GamesScrollListProps) => {
  const scrollRef = useRef<HTMLDivElement>(null);

  const virtualizer = useVirtualizer({
    count: sorted.length,
    estimateSize: () => ROW_HEIGHT,
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
              const game = sorted[virtualRow.index];

              return (
                <Box
                  height={virtualRow.size}
                  key={game.appId}
                  left={0}
                  position={'absolute'}
                  top={0}
                  transform={`translateY(${virtualRow.start}px)`}
                  width={'full'}
                >
                  <GameRow
                    game={game}
                    isConnecting={isConnecting}
                    isSelected={multiSelect.isSelected(game.appId.toString())}
                    onBulkUnlockSelected={onBulkUnlockSelected}
                    onOpen={() => onOpen(game.appId)}
                    onSelectAll={() => multiSelect.selectAll(allVisibleIds)}
                    onSelectNone={() => multiSelect.deselectAll()}
                    selectedCount={multiSelect.selectedCount}
                    onBulkUnlock={() =>
                      onBulkUnlockSingle(game.appId, game.name)
                    }
                    onSelect={(ctrlKey, shiftKey) =>
                      multiSelect.toggleSelect(
                        game.appId.toString(),
                        virtualRow.index,
                        ctrlKey,
                        shiftKey,
                        allVisibleIds,
                      )
                    }
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
