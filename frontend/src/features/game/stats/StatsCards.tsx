import type { StatSortField } from '../types';

import { Center, Text, VStack } from '@chakra-ui/react';
import { StatEditingProvider } from '@providers/stat-editing/StatEditingProvider';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { FilterEmptyState } from '@shared/components/FilterEmptyState';
import { ListToolbar } from '@shared/components/ListToolbar';
import { useRefreshAction } from '@shared/hooks/useRefreshAction';
import { useSort } from '@shared/hooks/useSort';
import { toaster } from '@shared/utils/toaster';
import { BarChart3 } from 'lucide-react';
import { useCallback, useMemo, useState } from 'react';

import { STATS_FILTER_OPTIONS } from '../constants';
import { ResetDialog } from './ResetDialog';
import { StatDrawer } from './StatDrawer';
import { StatsActionBar } from './StatsActionBar';
import { StatsScrollList } from './StatsScrollList';
import { StatsTableHeader } from './StatsTableHeader';

interface StatsCardsProps {
  loading: boolean;
}

export const StatsCards = ({ loading }: StatsCardsProps) => {
  const { stats, resetStats, refreshAchievements, refreshStats } =
    useSteamHub();

  const [filters, setFilters] = useState<Set<string>>(new Set());
  const [search, setSearch] = useState('');
  const [resetOpen, setResetOpen] = useState(false);
  const { sortDir, sortField, toggleSort } = useSort<StatSortField>('name');

  const refreshFn = useCallback(
    () => Promise.all([refreshAchievements(), refreshStats()]),
    [refreshAchievements, refreshStats],
  );
  const { handleRefresh, isRefreshing } = useRefreshAction(refreshFn);

  const doReset = useCallback(async () => {
    setResetOpen(false);
    const ok = await resetStats(false);
    if (ok) {
      toaster.create({
        title: 'Reset',
        description: 'Stats have been reset',
      });
      await Promise.all([refreshAchievements(), refreshStats()]);
    } else {
      toaster.create({
        title: 'Error',
        description: 'Failed to reset',
        type: 'error',
      });
    }
  }, [resetStats, refreshAchievements, refreshStats]);

  const filtered = useMemo(() => {
    let result = stats;
    if (filters.size > 0) {
      result = result.filter((s) => {
        if (filters.has('protected') && s.isProtected) {
          return true;
        }
        if (filters.has('unprotected') && !s.isProtected) {
          return true;
        }

        return false;
      });
    }
    if (search.trim()) {
      const q = search.toLowerCase();
      result = result.filter(
        (s) =>
          (s.name || s.id).toLowerCase().includes(q) ||
          s.id.toLowerCase().includes(q),
      );
    }

    return result;
  }, [stats, filters, search]);

  const sorted = useMemo(() => {
    const list = [...filtered];
    list.sort((a, b) => {
      let cmp = 0;
      switch (sortField) {
        case 'name':
          cmp = (a.name || a.id).localeCompare(b.name || b.id);
          break;
        case 'value':
          cmp = a.value - b.value;
          break;
      }

      return sortDir === 'asc' ? cmp : -cmp;
    });

    return list;
  }, [filtered, sortField, sortDir]);

  if (!loading && stats.length === 0) {
    return (
      <FilterEmptyState
        icon={<BarChart3 />}
        title={'No stats found for this game.'}
      />
    );
  }

  return (
    <StatEditingProvider>
      <VStack align={'stretch'} flex={1} gap={'2'} minH={0}>
        <ListToolbar
          activeFilters={filters}
          disabled={loading}
          filteredCount={filtered.length}
          filterOptions={STATS_FILTER_OPTIONS}
          isRefreshing={isRefreshing}
          onFiltersChange={setFilters}
          onRefresh={handleRefresh}
          onSearchChange={setSearch}
          placeholder={'Search stats...'}
          search={search}
          totalCount={stats.length}
        />

        {loading ? (
          <Center flex={1}>
            <Text color={'fg.muted'} fontSize={'sm'}>
              {'Loading stats...'}
            </Text>
          </Center>
        ) : filtered.length === 0 ? (
          <FilterEmptyState
            description={'Try adjusting your search or filters.'}
            title={'No stats match your search.'}
          />
        ) : (
          <>
            <StatsTableHeader
              onSort={toggleSort}
              sortDir={sortDir}
              sortField={sortField}
            />
            <StatsScrollList filtered={sorted} />
          </>
        )}

        <StatDrawer stats={stats} />
      </VStack>

      <StatsActionBar onReset={() => setResetOpen(true)} open={!loading} />

      <ResetDialog
        onClose={() => setResetOpen(false)}
        onReset={doReset}
        open={resetOpen}
      />
    </StatEditingProvider>
  );
};
