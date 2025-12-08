# SAM-HC (Steam Achievement Manager)

[![Build Status](https://github.com/pfrancug/SteamAchievementManager/actions/workflows/build.yml/badge.svg)](https://github.com/pfrancug/SteamAchievementManager/actions/workflows/build.yml)
[![Release](https://github.com/pfrancug/SteamAchievementManager/actions/workflows/release.yml/badge.svg)](https://github.com/pfrancug/SteamAchievementManager/releases/latest)

SAM-HC is an enhanced fork of Steam Achievement Manager (SAM), a lightweight application used to manage achievements and statistics for Steam. This application requires the [Steam client](https://store.steampowered.com/about/), a Steam account, and network access. Steam must be running and the user must be logged in.

## Download & Installation

**[Download Latest Release](https://github.com/pfrancug/SteamAchievementManager/releases/latest)**

### Installer Version (Recommended)

- Download `SAM-HC-Installer-v8.0.0.exe`
- Run the installer and follow the prompts
- Desktop shortcut is optional during installation
- Windows Defender may show a warning (click "More info" → "Run anyway")

### Portable Version

- Download `SAM-HC-Portable-v8.0.0.zip`
- Extract to any folder
- **Important:** Right-click extracted files → Properties → Check "Unblock" → Apply
- Or run PowerShell in the folder: `Get-ChildItem -Recurse | Unblock-File`
- Run `SAM-HC.Picker.exe`

## What's New in Version 8.0.0

### 🎨 Dark Mode & UI Refinements

- Converted entire UI to modern dark theme
- Optimized window layouts for better readability
- Replaced Fugue Icons with Google Fonts icons
- Polished interface design and consistency

### 🔓 Bulk Operations

- Enhanced Lock All, Unlock All, and Invert All to respect protected items
- Added Unlock All Games feature in Picker for batch achievement unlocking across multiple games

### 🔒 Protected Items

- Improved visual indicators for protected achievements/statistics
- Fixed missing protection checks in bulk operations and statistics modifications
- Protected items properly skipped and clearly visible

### 🏗️ Technical

- GitHub Actions CI/CD workflow
- Code cleanup and modernization

## License & Attribution

This is a fork of the original Steam Achievement Manager by Rick (Gibbed).

**Original Work:**

- Copyright © 2024 Rick (rick 'at' gibbed 'dot' us)
- Original repository: [gibbed/SteamAchievementManager](https://github.com/gibbed/SteamAchievementManager)
- Originally released as closed-source in 2008, open-sourced in 2024

**This Fork:**

- Copyright © 2025 Piotr Francug - HotCode
- Version 8.0.0 with significant enhancements and modifications

This software is licensed under the **zlib License**. See [LICENSE](LICENSE) for details.

## Icons

- **UI Icons**: From [Google Fonts Material Symbols](https://fonts.google.com/icons). See [Resources/ui/LICENSE](Resources/ui/LICENSE) for details.

## About

SAM-HC is developed and maintained by **Piotr Francug - HotCode**. If you find this tool useful and would like to support its development, consider buying me a coffee! ☕

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://buymeacoffee.com/pfrancug)

Your support helps keep this project alive and enables future improvements!
