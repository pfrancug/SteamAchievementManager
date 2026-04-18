import type { KeyboardEvent, ReactNode } from 'react';

import {
  CloseButton,
  HStack,
  IconButton,
  Input,
  InputGroup,
} from '@chakra-ui/react';
import { Counter } from '@shared/components/Counter';
import { ToggleFilterBar } from '@shared/components/ToggleFilterBar';
import { Tooltip } from '@shared/components/ui/tooltip';
import { RefreshCw, Search } from 'lucide-react';

interface FilterOption {
  label: string;
  value: string;
}

interface ListToolbarProps {
  activeFilters: Set<string>;
  children?: ReactNode;
  disabled: boolean;
  filteredCount: number;
  filterOptions: FilterOption[];
  isRefreshing: boolean;
  onFiltersChange: (filters: Set<string>) => void;
  onRefresh: () => void;
  onSearchChange: (value: string) => void;
  onSearchKeyDown?: (e: KeyboardEvent) => void;
  placeholder: string;
  search: string;
  totalCount: number;
}

export const ListToolbar = ({
  activeFilters,
  children,
  disabled,
  filteredCount,
  filterOptions,
  isRefreshing,
  onFiltersChange,
  onRefresh,
  onSearchChange,
  onSearchKeyDown,
  placeholder,
  search,
  totalCount,
}: ListToolbarProps) => (
  <>
    <HStack gap={'2'}>
      <InputGroup
        flex={1}
        startElement={<Search size={14} />}
        endElement={
          !disabled && search.length > 0 ? (
            <CloseButton
              me={'-1'}
              size={'2xs'}
              onClick={() => {
                onSearchChange('');
              }}
            />
          ) : undefined
        }
      >
        <Input
          bg={'bg.card'}
          borderColor={'border.card'}
          disabled={disabled}
          onChange={(e) => onSearchChange(e.currentTarget.value)}
          onKeyDown={onSearchKeyDown}
          placeholder={placeholder}
          size={'xs'}
          value={search}
        />
      </InputGroup>

      {children}

      <Tooltip label={'Refresh'}>
        <IconButton
          aria-label={'Refresh'}
          borderColor={'border.card'}
          disabled={disabled || isRefreshing}
          onClick={onRefresh}
          size={'xs'}
          variant={'outline'}
        >
          <RefreshCw
            size={14}
            style={
              disabled || isRefreshing
                ? { animation: 'spin 1s linear infinite' }
                : undefined
            }
          />
        </IconButton>
      </Tooltip>
    </HStack>

    <HStack justify={'space-between'}>
      <ToggleFilterBar
        activeFilters={activeFilters}
        disabled={disabled}
        onChange={onFiltersChange}
        options={filterOptions}
      />

      {!disabled && <Counter x={filteredCount} y={totalCount} />}
    </HStack>
  </>
);
