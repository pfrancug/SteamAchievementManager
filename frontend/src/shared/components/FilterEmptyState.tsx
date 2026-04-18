import type { ReactNode } from 'react';

import { EmptyState, VStack } from '@chakra-ui/react';
import { SearchX } from 'lucide-react';

interface FilterEmptyStateProps {
  description?: string;
  icon?: ReactNode;
  title: string;
}

export const FilterEmptyState = ({
  description,
  icon,
  title,
}: FilterEmptyStateProps) => {
  return (
    <EmptyState.Root>
      <EmptyState.Content>
        <EmptyState.Indicator>{icon ?? <SearchX />}</EmptyState.Indicator>
        <VStack textAlign={'center'}>
          <EmptyState.Title>{title}</EmptyState.Title>
          {description && (
            <EmptyState.Description>{description}</EmptyState.Description>
          )}
        </VStack>
      </EmptyState.Content>
    </EmptyState.Root>
  );
};
