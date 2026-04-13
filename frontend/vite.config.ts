import { resolve } from 'path';

import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

export default defineConfig({
  plugins: [react()],
  base: './',
  resolve: {
    alias: {
      '@app': resolve(__dirname, 'src/app'),
      '@features': resolve(__dirname, 'src/features'),
      '@providers': resolve(__dirname, 'src/providers'),
      '@services': resolve(__dirname, 'src/services'),
      '@shared': resolve(__dirname, 'src/shared'),
      '@theme': resolve(__dirname, 'src/theme'),
    },
  },
  server: { port: 5174 },
  build: { chunkSizeWarningLimit: 1000 },
});
