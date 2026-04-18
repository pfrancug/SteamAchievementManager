import js from '@eslint/js';
import react from 'eslint-plugin-react';
import reactHooks from 'eslint-plugin-react-hooks';
import reactRefresh from 'eslint-plugin-react-refresh';
import simpleImportSort from 'eslint-plugin-simple-import-sort';
import { defineConfig } from 'eslint/config';
import globals from 'globals';
import tseslint from 'typescript-eslint';
import { fileURLToPath } from 'url';
import { dirname } from 'path';

const __dirname = dirname(fileURLToPath(import.meta.url));

export default defineConfig([
  {
    files: ['src/**/*.{ts,tsx}'],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      ecmaVersion: 'latest',
      globals: globals.browser,
      parserOptions: {
        project: './tsconfig.app.json',
        tsconfigRootDir: __dirname,
      },
    },
    plugins: { react, 'simple-import-sort': simpleImportSort },
    rules: {
      'react-hooks/incompatible-library': 'off',
      curly: ['warn', 'all'],
      'func-style': ['warn', 'expression'],
      'padding-line-between-statements': [
        'warn',
        { blankLine: 'always', prev: '*', next: 'return' },
      ],
      'react/jsx-curly-brace-presence': [
        'warn',
        { props: 'always', propElementValues: 'always', children: 'always' },
      ],
      'react/jsx-sort-props': [
        'warn',
        {
          callbacksLast: false,
          ignoreCase: true,
          multiline: 'last',
          noSortAlphabetically: false,
          reservedFirst: false,
          shorthandFirst: true,
        },
      ],
      'react/self-closing-comp': 'warn',
      'simple-import-sort/exports': 'warn',
      'simple-import-sort/imports': [
        'warn',
        {
          groups: [
            // Type imports first
            ['^.*\\u0000$'],
            // External packages
            ['^@?\\w'],
            // Internal/absolute imports
            ['^'],
            // Relative imports
            ['^\\.'],
            // Side-effect imports (css, etc.) last
            ['^\\u0000'],
          ],
        },
      ],
    },
  },
]);
