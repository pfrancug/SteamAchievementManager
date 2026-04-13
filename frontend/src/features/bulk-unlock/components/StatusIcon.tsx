import type { StatusIconProps } from '../types';

import { Icon, Spinner } from '@chakra-ui/react';
import { AlertTriangle, Check, Clock, X } from 'lucide-react';

export const StatusIcon = ({ status }: StatusIconProps) => {
  switch (status) {
    case 'done':
      return (
        <Icon color={'green.400'} size={'sm'}>
          <Check />
        </Icon>
      );
    case 'failed':
      return (
        <Icon color={'red.400'} size={'sm'}>
          <AlertTriangle />
        </Icon>
      );
    case 'in-progress':
      return <Spinner color={'blue.400'} size={'sm'} />;
    case 'skipped':
      return (
        <Icon color={'gray.400'} size={'sm'}>
          <X />
        </Icon>
      );
    default:
      return (
        <Icon color={'fg.muted'} size={'sm'}>
          <Clock />
        </Icon>
      );
  }
};
