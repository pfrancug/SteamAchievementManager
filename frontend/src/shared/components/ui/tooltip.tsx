import type { ReactNode } from 'react';

import { Portal, Tooltip as ChakraTooltip } from '@chakra-ui/react';

export const Tooltip = ({
  label,
  children,
  disabled,
}: {
  label: ReactNode;
  children: ReactNode;
  disabled?: boolean;
}) => {
  if (disabled) {
    return <>{children}</>;
  }

  return (
    <ChakraTooltip.Root closeDelay={100} openDelay={300}>
      <ChakraTooltip.Trigger asChild>{children}</ChakraTooltip.Trigger>
      <Portal>
        <ChakraTooltip.Positioner>
          <ChakraTooltip.Content>{label}</ChakraTooltip.Content>
        </ChakraTooltip.Positioner>
      </Portal>
    </ChakraTooltip.Root>
  );
};
