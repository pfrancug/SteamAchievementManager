import type { GamesGridProps, SortField } from '../types';
import type { KeyboardEvent } from 'react';

import { Center, Text, VStack } from '@chakra-ui/react';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { FilterEmptyState } from '@shared/components/FilterEmptyState';
import { ListToolbar } from '@shared/components/ListToolbar';
import { useRefreshAction } from '@shared/hooks/useRefreshAction';
import { useSort } from '@shared/hooks/useSort';
import { useCallback, useEffect, useMemo, useState } from 'react';

import { GAME_TYPE_FILTERS } from '../constants';
import { GamesScrollList } from './GamesScrollList';
import { GamesTableHeader } from './GamesTableHeader';

export const GamesGrid = ({
  multiSelect,
  onBulkUnlockSelected,
  onBulkUnlockSingle,
  onOpen,
  onRefresh,
  onVisibleIdsChange,
}: GamesGridProps) => {
  const { games, gamesLoading, hubReady, isConnecting } = useSteamHub();
  const loading = !hubReady || gamesLoading || games.length === 0;

  const [search, setSearch] = useState('');
  const [typeFilters, setTypeFilters] = useState<Set<string>>(new Set());
  const { sortDir, sortField, toggleSort } = useSort<SortField>('name');
  const { handleRefresh, isRefreshing } = useRefreshAction(onRefresh);

  const filtered = useMemo(() => {
    let result = games;
    if (typeFilters.size > 0) {
      result = result.filter((g) => typeFilters.has(g.type));
    }
    if (search.trim()) {
      const q = search.toLowerCase();
      result = result.filter(
        (g) => g.name.toLowerCase().includes(q) || String(g.appId).includes(q),
      );
    }

    return result;
  }, [games, search, typeFilters]);

  const sorted = useMemo(() => {
    const list = [...filtered];
    list.sort((a, b) => {
      let cmp = 0;
      switch (sortField) {
        case 'name':
          cmp = a.name.localeCompare(b.name);
          break;
        case 'purchased': {
          cmp = (a.purchaseTimestamp ?? 0) - (b.purchaseTimestamp ?? 0);
          break;
        }
        case 'appId':
          cmp = a.appId - b.appId;
          break;
      }

      return sortDir === 'asc' ? cmp : -cmp;
    });

    return list;
  }, [filtered, sortField, sortDir]);

  const allVisibleIds = useMemo(
    () => sorted.map((g) => String(g.appId)),
    [sorted],
  );

  const handleSearchKeyDown = useCallback(
    (e: KeyboardEvent) => {
      if (e.key !== 'Enter') {
        return;
      }
      const id = Number(search.trim());
      if (id > 0) {
        onOpen(id);
      } else if (sorted.length === 1) {
        onOpen(sorted[0].appId);
      }
    },
    [search, sorted, onOpen],
  );

  useEffect(() => {
    onVisibleIdsChange(allVisibleIds);
  }, [allVisibleIds, onVisibleIdsChange]);

  return (
    <VStack align={'stretch'} flex={1} gap={'2'} minH={0}>
      <ListToolbar
        activeFilters={typeFilters}
        disabled={loading}
        filteredCount={sorted.length}
        filterOptions={GAME_TYPE_FILTERS}
        isRefreshing={isRefreshing}
        onFiltersChange={setTypeFilters}
        onRefresh={handleRefresh}
        onSearchChange={setSearch}
        onSearchKeyDown={handleSearchKeyDown}
        placeholder={'Search games by name or app ID...'}
        search={search}
        totalCount={games.length}
      />

      {loading ? (
        <Center flex={1}>
          <Text color={'fg.muted'} fontSize={'sm'}>
            {hubReady ? 'Loading games...' : 'Connecting to Steam...'}
          </Text>
        </Center>
      ) : sorted.length === 0 ? (
        <FilterEmptyState
          description={'Try adjusting your search or filters.'}
          title={'No games match your search.'}
        />
      ) : (
        <>
          <GamesTableHeader
            onSort={toggleSort}
            sortDir={sortDir}
            sortField={sortField}
          />
          <GamesScrollList
            allVisibleIds={allVisibleIds}
            isConnecting={isConnecting}
            multiSelect={multiSelect}
            onBulkUnlockSelected={onBulkUnlockSelected}
            onBulkUnlockSingle={onBulkUnlockSingle}
            onOpen={onOpen}
            sorted={sorted}
          />
        </>
      )}
    </VStack>
  );
};
