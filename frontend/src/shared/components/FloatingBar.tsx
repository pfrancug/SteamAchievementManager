import type { ReactNode } from 'react';

import { Box, HStack } from '@chakra-ui/react';

interface FloatingBarProps {
  children: ReactNode;
}

export const FloatingBar = ({ children }: FloatingBarProps) => (
  <Box
    backdropFilter={'blur(12px)'}
    bg={'bg.floating'}
    borderColor={'border.card'}
    borderTopWidth={'1px'}
    bottom={0}
    flexShrink={0}
    h={'56px'}
    left={0}
    position={'sticky'}
    px={'4'}
    right={0}
  >
    <HStack align={'center'} h={'100%'} justify={'space-between'}>
      {children}
    </HStack>
  </Box>
);
