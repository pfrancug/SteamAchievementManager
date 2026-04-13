import type { GamesTableHeaderProps } from '../types';

import { Box, Flex, HStack } from '@chakra-ui/react';
import { sortIcon } from '@shared/utils/sortIcon';

export const GamesTableHeader = ({
  onSort,
  sortDir,
  sortField,
}: GamesTableHeaderProps) => (
  <Box
    bg={'black'}
    borderBottomWidth={'1px'}
    borderColor={'border.card'}
    color={'fg.muted'}
    display={'grid'}
    fontSize={'xs'}
    fontWeight={'semibold'}
    gap={4}
    gridTemplateColumns={'160px 1fr 150px 150px'}
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
      onClick={() => onSort('purchased')}
      py={'2'}
      textAlign={'right'}
    >
      <Flex gap={'1'} justify={'flex-end'}>
        <span>{'Purchased'}</span>
        {sortIcon('purchased', sortField, sortDir)}
      </Flex>
    </Box>

    <Box
      cursor={'pointer'}
      onClick={() => onSort('appId')}
      py={'2'}
      textAlign={'right'}
    >
      <Flex gap={'1'} justify={'flex-end'}>
        <span>{'App ID'}</span>
        {sortIcon('appId', sortField, sortDir)}
      </Flex>
    </Box>
  </Box>
);
