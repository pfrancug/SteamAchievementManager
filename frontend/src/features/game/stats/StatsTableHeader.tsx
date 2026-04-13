import type { StatsTableHeaderProps } from '../types';

import { Box, Flex, HStack } from '@chakra-ui/react';
import { sortIcon } from '@shared/utils/sortIcon';

export const StatsTableHeader = ({
  onSort,
  sortDir,
  sortField,
}: StatsTableHeaderProps) => (
  <Box
    bg={'black'}
    borderBottomWidth={'1px'}
    borderColor={'border.card'}
    color={'fg.muted'}
    display={'grid'}
    fontSize={'xs'}
    fontWeight={'semibold'}
    gap={4}
    gridTemplateColumns={'1fr auto auto'}
    position={'sticky'}
    pr={4}
    top={0}
    zIndex={1}
  >
    <Box cursor={'pointer'} onClick={() => onSort('name')} py={'2'}>
      <HStack gap={'1'}>
        <span>{'Name'}</span>
        {sortIcon('name', sortField, sortDir)}
      </HStack>
    </Box>

    <Box
      cursor={'pointer'}
      onClick={() => onSort('value')}
      px={'4'}
      py={'2'}
      textAlign={'right'}
    >
      <Flex gap={'1'} justify={'flex-end'}>
        <span>{'Value'}</span>
        {sortIcon('value', sortField, sortDir)}
      </Flex>
    </Box>
  </Box>
);
