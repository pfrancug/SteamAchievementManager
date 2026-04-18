import type { SortDir } from '@shared/types/sort';

import { Icon } from '@chakra-ui/react';
import { ArrowDownWideNarrow, ArrowUpNarrowWide } from 'lucide-react';

export const sortIcon = <T extends string>(
  field: T,
  activeField: T,
  dir: SortDir,
) => {
  if (activeField !== field) {
    return null;
  }

  return (
    <Icon size={'sm'}>
      {dir === 'asc' ? <ArrowUpNarrowWide /> : <ArrowDownWideNarrow />}
    </Icon>
  );
};
