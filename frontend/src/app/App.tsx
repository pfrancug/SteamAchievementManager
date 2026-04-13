import type { AppProps } from './types';

import { Box, Flex } from '@chakra-ui/react';
import { BulkUnlock } from '@features/bulk-unlock/BulkUnlock';
import { Game } from '@features/game/Game';
import { Games } from '@features/games/Games';
import { TitleBar } from '@shared/components/TitleBar';
import { TITLES } from '@shared/constants/constants';
import { useEffect } from 'react';

const getContent = (mode: AppProps['mode']) => {
  switch (mode) {
    case 'game':
      return <Game />;
    case 'bulk-unlock':
      return <BulkUnlock />;
    default:
      return <Games />;
  }
};

export const App = ({ mode, gameName }: AppProps) => {
  const title =
    mode === 'game' && gameName
      ? `SAM-HC — ${gameName}`
      : (TITLES[mode] ?? 'SAM-HC');

  useEffect(() => {
    document.title = title;
  }, [title]);

  return (
    <Flex
      bg={'bg.surface'}
      direction={'column'}
      h={'100dvh'}
      overflow={'hidden'}
    >
      <TitleBar title={title} />
      <Box
        display={'flex'}
        flex={1}
        flexDirection={'column'}
        overflow={'hidden'}
      >
        {getContent(mode)}
      </Box>
    </Flex>
  );
};
