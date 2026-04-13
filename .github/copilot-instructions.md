# Copilot Instructions

## Project Overview

Steam Achievement Manager (SAM-HC) — an Electron + .NET 10 desktop app for managing Steam game achievements and statistics. Modernized fork of the original WinForms SAM.

## Architecture

```
frontend/  — Vite + React 19 + Chakra UI v3 frontend (TypeScript)
desktop/   — Electron shell (TypeScript, spawns .NET backend)
backend/   — .NET 10 backend (SignalR hub, Steam API via P/Invoke, win-x64)
shared/    — Shared TypeScript types for SignalR hub contracts
```

**Communication flow:** Electron spawns the .NET backend → backend prints `PORT:{n}` to stdout → frontend connects to SignalR hub at `http://localhost:{port}/hub`.

## Commands

Always use `npm run <script>` — never invoke eslint, prettier, or tsc via npx directly.

```bash
# Development
npm run dev                  # Build .NET + launch Vite dev server + Electron together
npm run dev:frontend         # Vite dev server only (port 5174)
npm run dev:desktop          # Compile + launch Electron only

# Build
npm run build:frontend       # Production Vite build
npm run build:desktop        # Compile Electron TypeScript
npm run build:shared         # Compile shared types

# Quality
npm run lint                 # ESLint all workspaces
npm run format               # Prettier format all files
npm run typecheck            # TypeScript check all workspaces

# .NET (run from backend/)
dotnet restore
dotnet build
dotnet publish -c Release    # Self-contained single-file (win-x64)
```

Workspace-specific scripts: `npm run <script> -w @sam/frontend`, `-w @sam/desktop`, `-w @sam/shared`.

## Code Style

- **TypeScript strict mode** — all strictness flags, no unused locals/params
- **Arrow functions only** (`func-style` eslint rule)
- **Named exports** — prefer `export const` over `export default`
- **Imports**: `type` keyword for type-only imports; sorted by simple-import-sort (types → external → internal → relative → side-effects)
- **Named imports only** — never use namespace/wildcard imports (`import { useState }` not `import * as React`)
- **JSX**: Always use curly braces for string props (`{'value'}` not `"value"`); sort props alphabetically (shorthand first)
- **Curly braces required** on all control-flow blocks
- **Blank line before return** statements

## File Organization

- **Feature-based architecture** — code is organized by feature under `src/features/` (e.g. `game/`, `games/`, `bulker/`)
- **One component per file** — each React component gets its own `.tsx` file, named after the component
- **Keep files short** — if a file exceeds ~300 lines, split it into smaller modules. Extract sub-components, helpers, constants, and types
- **Types colocated per feature** — each feature has its own `types.ts` for interfaces and types used within that feature. Cross-cutting types go in `src/shared/types/`
- **Shared code in `shared/`** — reusable components (`shared/components/`), hooks (`shared/hooks/`), utils (`shared/utils/`), constants (`shared/constants/`), and types (`shared/types/`)
- **Providers in `providers/`** — React context providers go in `src/providers/`, one subdirectory per provider (e.g. `steam-hub/`, `game-hub/`, `achievement-actions/`, `stat-editing/`)
- **Services in `services/`** — SignalR and other backend-facing helpers go in `src/services/`
- **App shell in `app/`** — root component and routing logic in `src/app/`

## Conventions

- **Path aliases**: `@app/*`, `@features/*`, `@providers/*`, `@services/*`, `@shared/*`, `@theme/*`
- **Naming**: PascalCase components/files, camelCase functions, SCREAMING_SNAKE for constants. Interfaces use natural suffixes (`Props`, `State`, `Info`) — no `I` prefix

## Frontend Stack

- **UI library**: Chakra UI v3 (`@chakra-ui/react`) — dark mode only, teal/cyan accent
- **Charts**: `@chakra-ui/charts` for data visualization
- **Icons**: `lucide-react` (Chakra-recommended)
- **Virtualization**: `@tanstack/react-virtual` for large lists/grids
- **Theme**: Custom Chakra theme in `frontend/src/theme/theme.ts` with semantic color tokens
- **Styling**: Chakra prop-based styling only — no CSS files, no inline style objects (except virtualization positioning)
- **Routing**: URL query parameters (`?mode=picker|game|bulk-unlock&appId=...&port=...`) — no client-side router

## .NET Conventions

- Target: `net10.0-windows`, platform: `win-x64` (loads `steamclient64.dll`)
- Self-contained single-file publish, no .NET runtime prerequisite for users
- P/Invoke vtable marshaling for Steam API interfaces

## Key Patterns

- **SignalR contracts** are defined in `shared/src/index.ts` — both frontend and backend must match these types
- **Protected achievements/stats** are read-only and must be skipped in bulk operations
- **Electron preload** exposes `electronAPI` — no direct Node.js APIs in renderer
- **Backend lifecycle**: Electron spawns .NET exe → waits for health check → SIGTERM on quit with 5s grace period
- **Multi-window**: Electron opens separate windows per game, each with its own .NET backend instance
- **Backend lifecycle**: Electron spawns .NET exe → waits for health check → SIGTERM on quit with 5s grace period
