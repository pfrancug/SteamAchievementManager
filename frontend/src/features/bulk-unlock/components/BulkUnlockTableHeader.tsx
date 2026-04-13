import type { BulkUnlockTableHeaderProps } from '../types';

import { Box, Flex, HStack } from '@chakra-ui/react';
import { sortIcon } from '@shared/utils/sortIcon';

export const BulkUnlockTableHeader = ({
  onSort,
  sortDir,
  sortField,
}: BulkUnlockTableHeaderProps) => (
  <Box
    bg={'black'}
    borderBottomWidth={'1px'}
    borderColor={'border.card'}
    color={'fg.muted'}
    display={'grid'}
    fontSize={'xs'}
    fontWeight={'semibold'}
    gap={4}
    gridTemplateColumns={'120px 1fr 60px 60px 60px 24px'}
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
      onClick={() => onSort('unlocked')}
      py={'2'}
      textAlign={'center'}
    >
      <Flex gap={'1'} justify={'center'}>
        <span>{'New'}</span>
        {sortIcon('unlocked', sortField, sortDir)}
      </Flex>
    </Box>

    <Box
      cursor={'pointer'}
      onClick={() => onSort('alreadyUnlocked')}
      py={'2'}
      textAlign={'center'}
    >
      <Flex gap={'1'} justify={'center'}>
        <span>{'Had'}</span>
        {sortIcon('alreadyUnlocked', sortField, sortDir)}
      </Flex>
    </Box>

    <Box
      cursor={'pointer'}
      onClick={() => onSort('protected')}
      py={'2'}
      textAlign={'center'}
    >
      <Flex gap={'1'} justify={'center'}>
        <span>{'Prot.'}</span>
        {sortIcon('protected', sortField, sortDir)}
      </Flex>
    </Box>

    <Box
      cursor={'pointer'}
      onClick={() => onSort('status')}
      py={'2'}
      textAlign={'center'}
    >
      <Flex gap={'1'} justify={'center'}>
        {sortIcon('status', sortField, sortDir)}
      </Flex>
    </Box>
  </Box>
);
