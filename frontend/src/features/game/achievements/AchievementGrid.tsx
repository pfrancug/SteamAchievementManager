import type { AchievementSortField } from '../types';

import { Center, Text, VStack } from '@chakra-ui/react';
import { useAchievementActions } from '@providers/achievement-actions/useAchievementActions';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { FilterEmptyState } from '@shared/components/FilterEmptyState';
import { ListToolbar } from '@shared/components/ListToolbar';
import { useRefreshAction } from '@shared/hooks/useRefreshAction';
import { useSort } from '@shared/hooks/useSort';
import { Trophy } from 'lucide-react';
import { useCallback, useEffect, useMemo, useState } from 'react';

import { ACHIEVEMENT_FILTER_OPTIONS } from '../constants';
import { matchesFilters } from '../utils';
import { AchievementScrollList } from './AchievementScrollList';
import { AchievementsTableHeader } from './AchievementsTableHeader';

interface AchievementGridProps {
  loading?: boolean;
}

export const AchievementGrid = ({ loading }: AchievementGridProps) => {
  const { achievements, refreshAchievements, refreshStats } = useSteamHub();
  const { setFilteredIds } = useAchievementActions();
  const [filters, setFilters] = useState<Set<string>>(new Set());
  const [search, setSearch] = useState('');
  const { sortDir, sortField, toggleSort } =
    useSort<AchievementSortField>('name');

  const refreshFn = useCallback(
    () => Promise.all([refreshAchievements(), refreshStats()]),
    [refreshAchievements, refreshStats],
  );
  const { handleRefresh, isRefreshing } = useRefreshAction(refreshFn);

  const filtered = useMemo(() => {
    let result = achievements;
    if (filters.size > 0) {
      result = result.filter((a) => matchesFilters(a, filters));
    }
    if (search.trim()) {
      const q = search.toLowerCase();
      result = result.filter(
        (a) =>
          a.name.toLowerCase().includes(q) ||
          a.description.toLowerCase().includes(q) ||
          a.id.toLowerCase().includes(q),
      );
    }

    return result;
  }, [achievements, filters, search]);

  const filteredIds = useMemo(() => filtered.map((a) => a.id), [filtered]);

  const sorted = useMemo(() => {
    const list = [...filtered];
    list.sort((a, b) => {
      let cmp = 0;
      switch (sortField) {
        case 'name':
          cmp = (a.name || a.id).localeCompare(b.name || b.id);
          break;
        case 'unlockDate': {
          const aTime = a.unlockTime ? new Date(a.unlockTime).getTime() : 0;
          const bTime = b.unlockTime ? new Date(b.unlockTime).getTime() : 0;
          cmp = aTime - bTime;
          break;
        }
      }

      return sortDir === 'asc' ? cmp : -cmp;
    });

    return list;
  }, [filtered, sortField, sortDir]);

  useEffect(() => {
    setFilteredIds(filteredIds);
  }, [filteredIds, setFilteredIds]);

  return (
    <VStack align={'stretch'} flex={1} gap={'2'} minH={0}>
      <ListToolbar
        activeFilters={filters}
        disabled={!!loading}
        filteredCount={filtered.length}
        filterOptions={ACHIEVEMENT_FILTER_OPTIONS}
        isRefreshing={isRefreshing}
        onFiltersChange={setFilters}
        onRefresh={handleRefresh}
        onSearchChange={setSearch}
        placeholder={'Search achievements...'}
        search={search}
        totalCount={achievements.length}
      />

      {loading ? (
        <Center flex={1}>
          <Text color={'fg.muted'} fontSize={'sm'}>
            {'Loading achievements...'}
          </Text>
        </Center>
      ) : achievements.length === 0 ? (
        <FilterEmptyState
          icon={<Trophy />}
          title={'No achievements found for this game.'}
        />
      ) : filtered.length === 0 ? (
        <FilterEmptyState
          description={'Try adjusting your search or filters.'}
          title={'No achievements match your search.'}
        />
      ) : (
        <>
          <AchievementsTableHeader
            onSort={toggleSort}
            sortDir={sortDir}
            sortField={sortField}
          />
          <AchievementScrollList filtered={sorted} filteredIds={filteredIds} />
        </>
      )}
    </VStack>
  );
};
