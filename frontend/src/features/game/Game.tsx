import { Box, For, Icon, Tabs, VStack } from '@chakra-ui/react';
import { useSteamHub } from '@providers/steam-hub/useSteamHub';
import { SteamErrorState } from '@shared/components/SteamErrorState';
import { BarChart3, Trophy } from 'lucide-react';
import { useState } from 'react';

import { AchievementsTab } from './achievements/AchievementsTab';
import { GameHeader } from './components/GameHeader';
import { StatsCards } from './stats/StatsCards';

export const Game = () => {
  const { status, games, isConnecting, isLoading } = useSteamHub();
  const [activeTab, setActiveTab] = useState<string | null>('achievements');

  const currentGame = games[0];
  const loading = isConnecting || isLoading;

  const tabs = [
    {
      icon: <Trophy />,
      label: 'Achievements',
      value: 'achievements',
    },
    {
      icon: <BarChart3 />,
      label: 'Stats',
      value: 'stats',
    },
  ];

  return (
    <Box
      display={'flex'}
      flex={1}
      flexDirection={'column'}
      minH={0}
      pos={'relative'}
    >
      <VStack align={'stretch'} flex={1} gap={'2'} minH={0} p={'4'} pb={'0'}>
        <GameHeader game={currentGame} status={status} />

        {status.error ? (
          <SteamErrorState />
        ) : (
          <Tabs.Root
            lazyMount
            unmountOnExit
            display={'flex'}
            flex={1}
            flexDirection={'column'}
            minH={0}
            onValueChange={(e) => setActiveTab(e.value)}
            value={activeTab}
            variant={'line'}
          >
            <Tabs.List>
              <For each={tabs}>
                {(tab) => (
                  <Tabs.Trigger key={tab.value} value={tab.value}>
                    <Icon size={'sm'}>{tab.icon}</Icon>
                    {tab.label}
                  </Tabs.Trigger>
                )}
              </For>
            </Tabs.List>

            <Tabs.Content
              display={'flex'}
              flex={1}
              flexDirection={'column'}
              minH={0}
              value={'achievements'}
            >
              <AchievementsTab loading={loading} />
            </Tabs.Content>

            <Tabs.Content
              display={'flex'}
              flex={1}
              flexDirection={'column'}
              minH={0}
              value={'stats'}
            >
              <StatsCards loading={loading} />
            </Tabs.Content>
          </Tabs.Root>
        )}
      </VStack>
    </Box>
  );
};
