import { Button, ButtonGroup, For, HStack } from '@chakra-ui/react';

interface ToggleFilterOption {
  label: string;
  value: string;
}

interface ToggleFilterBarProps {
  options: ToggleFilterOption[];
  activeFilters: Set<string>;
  onChange: (filters: Set<string>) => void;
  disabled?: boolean;
}

export const ToggleFilterBar = ({
  options,
  activeFilters,
  onChange,
  disabled,
}: ToggleFilterBarProps) => {
  const allActive = activeFilters.size === 0;

  const handleToggleAll = () => {
    onChange(new Set());
  };

  const handleToggle = (value: string) => {
    if (activeFilters.has(value)) {
      onChange(new Set());
    } else {
      onChange(new Set([value]));
    }
  };

  return (
    <HStack>
      <Button
        colorPalette={allActive ? 'blue' : 'white'}
        disabled={disabled}
        onClick={handleToggleAll}
        size={'2xs'}
        variant={allActive ? 'solid' : 'subtle'}
      >
        {'All'}
      </Button>

      <ButtonGroup>
        <For each={options}>
          {(option) => (
            <Button
              colorPalette={activeFilters.has(option.value) ? 'blue' : 'white'}
              disabled={disabled}
              key={option.value}
              onClick={() => handleToggle(option.value)}
              size={'2xs'}
              variant={activeFilters.has(option.value) ? 'solid' : 'subtle'}
            >
              {option.label}
            </Button>
          )}
        </For>
      </ButtonGroup>
    </HStack>
  );
};
