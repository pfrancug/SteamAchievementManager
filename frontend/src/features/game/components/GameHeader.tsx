import type { GameHeaderProps } from '../types';

import { HStack, Image, Tag, Text, VStack } from '@chakra-ui/react';
import { Tooltip } from '@shared/components/ui/tooltip';
import { Hash, ShoppingBasket, Swords } from 'lucide-react';

import { GAME_TYPE_LABELS } from '../constants';

export const GameHeader = ({ game, status }: GameHeaderProps) => {
  return (
    <HStack gap={'2'}>
      {game?.imageUrl && (
        <Image
          borderRadius={'md'}
          flexShrink={0}
          h={'60px'}
          objectFit={'cover'}
          src={game.imageUrl}
          w={'160px'}
        />
      )}
      <VStack align={'start'} gap={'unset'} h={'60px'} justify={'space-evenly'}>
        <Text fontSize={'xl'} fontWeight={'semibold'}>
          {status.gameName || game?.name}
        </Text>

        <HStack>
          {game?.appId && (
            <Tooltip label={`App ID: ${game.appId}`}>
              <Tag.Root variant={'outline'}>
                <Tag.StartElement>
                  <Hash />
                </Tag.StartElement>

                <Tag.Label>{game.appId}</Tag.Label>
              </Tag.Root>
            </Tooltip>
          )}

          {game?.purchaseTimestamp && (
            <Tooltip
              label={`Type: ${GAME_TYPE_LABELS[game.type] || 'Unknown'}`}
            >
              <Tag.Root variant={'outline'}>
                <Tag.StartElement>
                  <Swords />
                </Tag.StartElement>

                <Tag.Label> {GAME_TYPE_LABELS[game.type]}</Tag.Label>
              </Tag.Root>
            </Tooltip>
          )}

          {game?.purchaseTimestamp && (
            <Tooltip
              label={`Purchase Date: ${new Date(game.purchaseTimestamp * 1000).toLocaleDateString()}`}
            >
              <Tag.Root variant={'outline'}>
                <Tag.StartElement>
                  <ShoppingBasket />
                </Tag.StartElement>

                <Tag.Label>
                  {new Date(game.purchaseTimestamp * 1000).toLocaleDateString()}
                </Tag.Label>
              </Tag.Root>
            </Tooltip>
          )}
        </HStack>
      </VStack>
    </HStack>
  );
};
