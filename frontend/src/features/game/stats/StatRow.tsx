import type { StatRowProps } from '../types';

import {
  Flex,
  Grid,
  GridItem,
  Icon,
  Menu,
  Portal,
  Separator,
  Text,
} from '@chakra-ui/react';
import { useStatEditing } from '@providers/stat-editing/useStatEditing';
import { ExternalLink, Shield } from 'lucide-react';
import { memo, useCallback } from 'react';

import { STATS_ROW_HEIGHT } from '../constants';

export const StatRow = memo(
  ({ displayValue, isActive, isSaving, stat }: StatRowProps) => {
    const { setActiveStatId } = useStatEditing();

    const handleToggle = useCallback(() => {
      setActiveStatId(isActive ? null : stat.id);
    }, [isActive, setActiveStatId, stat.id]);

    const handleOpenDetails = useCallback(() => {
      setActiveStatId(stat.id);
    }, [setActiveStatId, stat.id]);

    return (
      <>
        <Menu.Root>
          <Menu.ContextTrigger asChild>
            <Grid
              _hover={{ bg: 'cyan.400/4' }}
              bg={isActive ? 'cyan.400/8' : 'transparent'}
              columnGap={4}
              cursor={'pointer'}
              minH={`${STATS_ROW_HEIGHT - 1}px`}
              onClick={handleToggle}
              pr={4}
              templateColumns={'1fr auto auto '}
              transition={'background 0.15s ease'}
            >
              {/* Name */}
              <GridItem>
                <Flex alignItems={'center'} h={'100%'}>
                  <Text fontSize={'sm'} fontWeight={'medium'} lineClamp={1}>
                    {stat.name || stat.id}
                  </Text>
                </Flex>
              </GridItem>

              {/* Value */}
              <GridItem alignItems={'center'} display={'flex'}>
                <Text
                  color={isSaving ? 'fg.muted' : 'fg'}
                  fontSize={'sm'}
                  fontVariantNumeric={'tabular-nums'}
                  textAlign={'right'}
                >
                  {displayValue}
                </Text>
              </GridItem>

              {/* Shield */}
              <GridItem alignItems={'center'} display={'flex'}>
                <Icon
                  color={'orange.400'}
                  size={'sm'}
                  visibility={stat.isProtected ? 'visible' : 'hidden'}
                >
                  <Shield />
                </Icon>
              </GridItem>
            </Grid>
          </Menu.ContextTrigger>

          <Portal>
            <Menu.Positioner>
              <Menu.Content>
                <Text color={'fg.muted'} fontSize={'xs'} px={'2'} py={'1'}>
                  {`Stat: ${stat.name || stat.id}`}
                </Text>

                <Menu.Item onSelect={handleOpenDetails} value={'open-details'}>
                  <ExternalLink size={14} />
                  <Menu.ItemText>{'Open Details'}</Menu.ItemText>
                </Menu.Item>
              </Menu.Content>
            </Menu.Positioner>
          </Portal>
        </Menu.Root>

        <Separator />
      </>
    );
  },
);
