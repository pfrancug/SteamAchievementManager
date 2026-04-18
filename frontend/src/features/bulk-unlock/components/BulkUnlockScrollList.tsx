import type { BulkUnlockSortField, GameStatus } from '../types';

import { Box, ScrollArea } from '@chakra-ui/react';
import { useBulkUnlockResults } from '@providers/bulk-unlock/useBulkUnlock';
import { useSort } from '@shared/hooks/useSort';
import { useVirtualizer } from '@tanstack/react-virtual';
import { useMemo, useRef } from 'react';

import { ROW_HEIGHT } from '../constants';
import { BulkUnlockRow } from './BulkUnlockRow';
import { BulkUnlockTableHeader } from './BulkUnlockTableHeader';

const STATUS_ORDER: Record<GameStatus, number> = {
  'in-progress': 0,
  pending: 1,
  done: 2,
  failed: 3,
  skipped: 4,
};

export const BulkUnlockScrollList = () => {
  const { getResult, resultCount } = useBulkUnlockResults();
  const scrollRef = useRef<HTMLDivElement>(null);
  const { sortDir, sortField, toggleSort } =
    useSort<BulkUnlockSortField>('name');

  const sortedIndices = useMemo(() => {
    const indices = Array.from({ length: resultCount }, (_, i) => i);

    indices.sort((a, b) => {
      const ra = getResult(a);
      const rb = getResult(b);
      let cmp = 0;

      switch (sortField) {
        case 'alreadyUnlocked':
          cmp = ra.alreadyUnlocked - rb.alreadyUnlocked;
          break;
        case 'name':
          cmp = ra.name.localeCompare(rb.name);
          break;
        case 'protected':
          cmp = ra.protected - rb.protected;
          break;
        case 'status':
          cmp = STATUS_ORDER[ra.status] - STATUS_ORDER[rb.status];
          break;
        case 'unlocked':
          cmp = ra.unlocked - rb.unlocked;
          break;
      }

      return sortDir === 'asc' ? cmp : -cmp;
    });

    return indices;
  }, [getResult, resultCount, sortDir, sortField]);

  const virtualizer = useVirtualizer({
    count: resultCount,
    estimateSize: () => ROW_HEIGHT,
    getScrollElement: () => scrollRef.current,
    paddingEnd: 88,
  });

  return (
    <>
      <BulkUnlockTableHeader
        onSort={toggleSort}
        sortDir={sortDir}
        sortField={sortField}
      />
      <ScrollArea.Root flex={1} minH={0} variant={'always'}>
        <ScrollArea.Viewport ref={scrollRef}>
          <ScrollArea.Content>
            <Box
              height={virtualizer.getTotalSize()}
              position={'relative'}
              width={'full'}
            >
              {virtualizer.getVirtualItems().map((virtualRow) => {
                const result = getResult(sortedIndices[virtualRow.index]);

                return (
                  <Box
                    height={virtualRow.size}
                    key={result.appId}
                    left={0}
                    position={'absolute'}
                    top={0}
                    transform={`translateY(${virtualRow.start}px)`}
                    width={'full'}
                  >
                    <BulkUnlockRow result={result} />
                  </Box>
                );
              })}
            </Box>
          </ScrollArea.Content>
        </ScrollArea.Viewport>
        <ScrollArea.Scrollbar bg={'transparent'} orientation={'vertical'} />
        <ScrollArea.Corner bg={'bg'} />
      </ScrollArea.Root>
    </>
  );
};
