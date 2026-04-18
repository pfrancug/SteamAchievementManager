import { Toast, Toaster as ChakraToaster } from '@chakra-ui/react';
import { toaster } from '@shared/utils/toaster';

export const Toaster = () => (
  <ChakraToaster toaster={toaster}>
    {(toast) => (
      <Toast.Root minW={'300px'}>
        <Toast.Title>{toast.title}</Toast.Title>
        {toast.description && (
          <Toast.Description>{toast.description}</Toast.Description>
        )}
        <Toast.CloseTrigger />
      </Toast.Root>
    )}
  </ChakraToaster>
);
