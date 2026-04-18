import type { AchievementsTableHeaderProps } from '../types';

import { Box, Flex, HStack } from '@chakra-ui/react';
import { sortIcon } from '@shared/utils/sortIcon';

export const AchievementsTableHeader = ({
  onSort,
  sortDir,
  sortField,
}: AchievementsTableHeaderProps) => (
  <Box
    bg={'black'}
    borderBottomWidth={'1px'}
    borderColor={'border.card'}
    color={'fg.muted'}
    display={'grid'}
    fontSize={'xs'}
    fontWeight={'semibold'}
    gap={4}
    gridTemplateColumns={'64px 1fr auto'}
    position={'sticky'}
    pr={4}
    top={0}
    zIndex={1}
  >
    <Box />

    <Box cursor={'pointer'} onClick={() => onSort('name')} py={'2'}>
      <HStack gap={'1'}>
        <span>{'Name'}</span>
        {sortIcon('name', sortField, sortDir)}
      </HStack>
    </Box>

    <Box
      cursor={'pointer'}
      onClick={() => onSort('unlockDate')}
      py={'2'}
      textAlign={'right'}
    >
      <Flex gap={'1'} justify={'flex-end'}>
        <span>{'Unlocked'}</span>
        {sortIcon('unlockDate', sortField, sortDir)}
      </Flex>
    </Box>
  </Box>
);
