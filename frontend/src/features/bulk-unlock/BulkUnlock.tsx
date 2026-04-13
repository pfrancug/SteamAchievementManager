import { Heading, Highlight, Separator, VStack } from '@chakra-ui/react';
import { useBulkUnlockProgress } from '@providers/bulk-unlock/useBulkUnlock';

import { BulkUnlockActionBar } from './components/BulkUnlockActionBar';
import { BulkUnlockChart } from './components/BulkUnlockChart';
import { BulkUnlockProgress } from './components/BulkUnlockProgress';
import { BulkUnlockScrollList } from './components/BulkUnlockScrollList';

export const BulkUnlock = () => {
  const { phase } = useBulkUnlockProgress();

  const STATUS_MAP: Record<string, { color: string; label: string }> = {
    cancelled: {
      color: 'gray',
      label: 'Cancelled',
    },
    done: {
      color: 'green',
      label: 'Done',
    },
    loading: {
      color: 'blue',
      label: 'Loading',
    },
    processing: {
      color: 'blue',
      label: 'Running',
    },
  };

  return (
    <VStack align={'stretch'} flex={1} gap={'3'} minH={0} p={'4'} pb={'0'}>
      <Heading size={'lg'}>
        <Highlight
          query={STATUS_MAP[phase].label}
          styles={{
            bg: `${STATUS_MAP[phase].color}.subtle`,
            color: `${STATUS_MAP[phase].color}.fg`,
          }}
        >{`Status: ${STATUS_MAP[phase].label}`}</Highlight>
      </Heading>
      <BulkUnlockProgress />
      <Separator />
      <Heading size={'lg'}>{'Achievements Breakdown'}</Heading>
      <BulkUnlockChart />
      <BulkUnlockScrollList />
      <BulkUnlockActionBar />
    </VStack>
  );
};
