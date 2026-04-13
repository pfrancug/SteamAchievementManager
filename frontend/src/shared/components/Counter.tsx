import { Tag } from '@chakra-ui/react';

interface CounterProps {
  x: number;
  y: number;
}

export const Counter = ({ x, y }: CounterProps) => (
  <Tag.Root size={'sm'} variant={'surface'}>
    <Tag.Label> {`${x} / ${y}`}</Tag.Label>
  </Tag.Root>
);
