# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2026-08-22

### Added
- Overview tab with per-type orphan counts and combined size on disk
- Settings tab with font scale and configurable common/custom filetype filters
- Settings persist to `ProjectSettings/OrphanCheckerSettings.json`

### Fixed
- Editing custom filetypes in the settings window no longer modifies unrelated entries
- "Selected To Trash" / "Delete Selected" now perform their labeled actions (handlers were swapped)

## [0.1.0] - 2026-08-20

### Added
- Initial release
- Scan project for orphaned assets (scripts, prefabs, materials)
- Show, delete, and trash individual orphans
- Bulk select and delete/trash orphans
- Configurable file type filters
- Font scale settings
