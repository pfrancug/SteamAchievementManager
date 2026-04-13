# <img src="assets/logo.png" width="32" height="32" /> SAM-HC (Steam Achievement Manager)

[![Build](https://github.com/pfrancug/SteamAchievementManager/actions/workflows/build.yml/badge.svg)](https://github.com/pfrancug/SteamAchievementManager/actions/workflows/build.yml)
[![Release](https://github.com/pfrancug/SteamAchievementManager/actions/workflows/release.yml/badge.svg)](https://github.com/pfrancug/SteamAchievementManager/releases/latest)
[![License](https://img.shields.io/badge/license-zlib-blue)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%20x64-blue)](https://github.com/pfrancug/SteamAchievementManager/releases/latest)

[![Electron](https://img.shields.io/badge/Electron-47848F?logo=electron&logoColor=white)](https://www.electronjs.org/)
[![React](https://img.shields.io/badge/React%2019-61DAFB?logo=react&logoColor=black)](https://react.dev/)
[![.NET](https://img.shields.io/badge/.NET%2010-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)

A modern desktop app for managing Steam game achievements and statistics. Portable single-file executable — [download](https://github.com/pfrancug/SteamAchievementManager/releases/latest) and run.

Works with the [Steam](https://store.steampowered.com/about/) client.

## Features

- **Game Library** — browse, search, and filter your Steam games
- **Achievement Manager** — lock, unlock, and toggle achievements per game
- **Statistics Editor** — edit integer, float, and rate stats with validation
- **Bulk Unlock** — batch unlock achievements across multiple games
- **Protected Items** — read-only achievements and stats are marked and skipped in bulk operations
- **Multi-Window** — each game opens in its own window with a dedicated backend instance

| Game Library                      | Achievement Manager                     | Bulk Unlock                     |
| --------------------------------- | --------------------------------------- | ------------------------------- |
| ![Game Library](assets/games.png) | ![Achievement Manager](assets/game.png) | ![Bulk Unlock](assets/bulk.png) |

## Architecture

Electron shell spawning a .NET 10 backend per game window, communicating via SignalR over localhost.

| Layer    | Stack                                                    |
| -------- | -------------------------------------------------------- |
| Frontend | Vite, React 19, Chakra UI v3, TypeScript                 |
| Backend  | .NET 10, SignalR, Steam API via P/Invoke, self-contained |
| Shell    | Electron, multi-window, per-game backend process         |
| Shared   | TypeScript types for SignalR hub contracts               |

## License

Licensed under **zlib**. See [LICENSE](LICENSE).

Fork of [gibbed/SteamAchievementManager](https://github.com/gibbed/SteamAchievementManager).

## About

Developed by **Piotr Francug - HotCode**.

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://buymeacoffee.com/pfrancug)
