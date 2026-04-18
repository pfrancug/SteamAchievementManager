import { useEffect, useState } from 'react';

export const useModifierKeys = (): boolean => {
  const [active, setActive] = useState(false);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      setActive(e.ctrlKey || e.metaKey || e.shiftKey);
    };
    const onBlur = () => setActive(false);
    window.addEventListener('keydown', onKey);
    window.addEventListener('keyup', onKey);
    window.addEventListener('blur', onBlur);

    return () => {
      window.removeEventListener('keydown', onKey);
      window.removeEventListener('keyup', onKey);
      window.removeEventListener('blur', onBlur);
    };
  }, []);

  return active;
};
