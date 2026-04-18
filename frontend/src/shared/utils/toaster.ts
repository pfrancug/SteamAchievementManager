import { createToaster } from '@chakra-ui/react';

export const toaster = createToaster({
  placement: 'bottom-end',
  pauseOnPageIdle: true,
  overlap: false,
  gap: 8,
  offsets: { bottom: '64px', right: '16px', left: '16px', top: '16px' },
});
