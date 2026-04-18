import type { CSSProperties } from 'react';

import { Flex, HStack, Image, Text } from '@chakra-ui/react';
import { Copy, Minus, Square, X } from 'lucide-react';
import { useEffect, useState } from 'react';

import logo from '/logo.png?url';

interface TitleBarProps {
  title: string;
}

export const TitleBar = ({ title }: TitleBarProps) => {
  const [isMaximized, setIsMaximized] = useState(false);
  const isElectron = !!window.electronAPI;

  useEffect(() => {
    if (!window.electronAPI) {
      return;
    }
    window.electronAPI.isMaximized().then(setIsMaximized);
    const cleanup = window.electronAPI.onMaximizeChange(setIsMaximized);

    return cleanup;
  }, []);

  const handleMinimize = () => void window.electronAPI?.minimizeWindow();
  const handleMaximize = () => void window.electronAPI?.maximizeWindow();
  const handleClose = () => void window.electronAPI?.closeWindow();

  return (
    <Flex
      align={'center'}
      bg={'bg.panel'}
      borderBottomWidth={'1px'}
      borderColor={'border.card'}
      flexShrink={0}
      h={'36px'}
      style={{ WebkitAppRegion: 'drag' } as CSSProperties}
    >
      <HStack flex={1} gap={'2'} px={'3'}>
        <Image flexShrink={0} h={'16px'} src={logo} w={'16px'} />
        <Text truncate color={'fg.muted'} fontSize={'xs'} fontWeight={'medium'}>
          {title}
        </Text>
      </HStack>

      {isElectron && (
        <HStack
          flexShrink={0}
          gap={'0'}
          style={{ WebkitAppRegion: 'no-drag' } as CSSProperties}
        >
          <Flex
            _hover={{ bg: 'whiteAlpha.100', color: 'fg' }}
            align={'center'}
            aria-label={'Minimize'}
            as={'button'}
            color={'fg.muted'}
            cursor={'pointer'}
            h={'36px'}
            justify={'center'}
            onClick={handleMinimize}
            transition={'colors'}
            w={'46px'}
          >
            <Minus size={14} />
          </Flex>
          <Flex
            _hover={{ bg: 'whiteAlpha.100', color: 'fg' }}
            align={'center'}
            aria-label={isMaximized ? 'Restore' : 'Maximize'}
            as={'button'}
            color={'fg.muted'}
            cursor={'pointer'}
            h={'36px'}
            justify={'center'}
            onClick={handleMaximize}
            transition={'colors'}
            w={'46px'}
          >
            {isMaximized ? <Copy size={12} /> : <Square size={12} />}
          </Flex>
          <Flex
            _hover={{ bg: 'red.500/80', color: 'white' }}
            align={'center'}
            aria-label={'Close'}
            as={'button'}
            borderTopRightRadius={'inherit'}
            color={'fg.muted'}
            cursor={'pointer'}
            h={'36px'}
            justify={'center'}
            onClick={handleClose}
            transition={'colors'}
            w={'46px'}
          >
            <X size={14} />
          </Flex>
        </HStack>
      )}
    </Flex>
  );
};
