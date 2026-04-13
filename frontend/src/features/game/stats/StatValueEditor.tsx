import type { StatValueEditorProps } from '../types';
import type { InputEvent } from 'react';

import { Editable, IconButton } from '@chakra-ui/react';
import { Check, Pencil, X } from 'lucide-react';
import { useCallback } from 'react';

export const StatValueEditor = ({
  displayValue,
  isSaving,
  onSave,
  stat: s,
}: StatValueEditorProps) => {
  const handleBeforeInput = useCallback(
    (e: InputEvent<HTMLInputElement>) => {
      const input = e.target as HTMLInputElement;
      const data = e.nativeEvent.data ?? '';
      const { selectionStart, selectionEnd, value } = input;
      const next =
        value.slice(0, selectionStart ?? 0) +
        data +
        value.slice(selectionEnd ?? 0);
      const pattern = s.type === 'int' ? /^-?\d*$/ : /^-?\d*\.?\d*$/;

      if (!pattern.test(next)) {
        e.preventDefault();
      }
    },
    [s.type],
  );

  return (
    <Editable.Root
      _hover={{ bg: 'transparent' }}
      activationMode={'none'}
      defaultValue={displayValue.toString()}
      disabled={isSaving}
      justifyContent={'flex-end'}
      key={displayValue}
      onValueCommit={({ value }) => onSave(s.id, s.type, value)}
      submitMode={'none'}
    >
      {/* Preview */}
      <Editable.Preview
        alignItems={'center'}
        justifyContent={'flex-end'}
        textAlign={'right'}
      />

      {/* Input */}
      <Editable.Input onBeforeInput={handleBeforeInput} textAlign={'right'} />

      <Editable.Control>
        {/* Hidden spacer for consistent width */}
        <Editable.EditTrigger
          asChild
          colorPalette={'gray'}
          visibility={'hidden'}
        >
          <IconButton size={'xs'} variant={'ghost'} />
        </Editable.EditTrigger>

        {/* Edit */}
        <Editable.EditTrigger asChild colorPalette={'gray'}>
          <IconButton size={'xs'} variant={'ghost'}>
            <Pencil />
          </IconButton>
        </Editable.EditTrigger>

        {/* Submit */}
        <Editable.SubmitTrigger asChild>
          <IconButton colorPalette={'green'} size={'xs'} variant={'ghost'}>
            <Check />
          </IconButton>
        </Editable.SubmitTrigger>

        {/* Cancel */}
        <Editable.CancelTrigger asChild colorPalette={'red'}>
          <IconButton size={'xs'} variant={'ghost'}>
            <X />
          </IconButton>
        </Editable.CancelTrigger>
      </Editable.Control>
    </Editable.Root>
  );
};
