import { createSystem, defaultConfig, defineConfig } from '@chakra-ui/react';

const config = defineConfig({
  theme: {
    keyframes: {
      slideInRight: {
        from: { opacity: 0, transform: 'translateX(12px)' },
        to: { opacity: 1, transform: 'translateX(0)' },
      },
      slideInLeft: {
        from: { opacity: 0, transform: 'translateX(-12px)' },
        to: { opacity: 1, transform: 'translateX(0)' },
      },
      fadeIn: {
        from: { opacity: 0 },
        to: { opacity: 1 },
      },
      spin: {
        from: { transform: 'rotate(0deg)' },
        to: { transform: 'rotate(360deg)' },
      },
    },
    tokens: {
      animations: {
        slideInRight: { value: 'slideInRight 200ms ease-out' },
        slideInLeft: { value: 'slideInLeft 200ms ease-out' },
        fadeIn: { value: 'fadeIn 200ms ease-out' },
      },
    },
  },
});

export const system = createSystem(defaultConfig, config);
