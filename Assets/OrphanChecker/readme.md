# Orphan Checker

Editor tool that scans your project for orphaned assets — assets that are no longer referenced by any scannable file (prefabs, scenes, materials, animators, etc.).

## Usage

Open the tool window via **Tools > Orphan Checker**. The window has three tabs.

### Overview

Lists the number of orphaned assets per type together with their combined size on disk.

### Main

- Click **Check** to scan the project. Orphans are listed grouped by asset type.
- Use **Show** next to an asset to highlight it in the Project window.
- Use **Trash** to move a single asset to the OS trash bin, or **Delete** to remove it permanently.
- Toggle multiple orphans, then use **Selected To Trash** / **Delete Selected** to clean up in bulk.
- **Select All** / **Clear Selected** toggle every entry at once.

### Settings

- **Font Scale** adjusts the size of the group headers in the Main tab.
- Under **Filetypes**, choose which asset types are scanned:
  - *Common*: built-in Unity asset types. Each row can be renamed, retargeted, or deactivated.
  - *Custom*: add your own search tokens (must start with `t:`) or remove them again.
- **Apply** persists all changes to `ProjectSettings/OrphanCheckerSettings.json`, so the scan configuration is shared with the project through version control.

## How it works

The scanner reads scannable asset files (scenes, prefabs, materials, animation controllers, …) as text and counts GUID references between them. Any asset that is never referenced by another scannable file is reported as an orphan. Scripts only count if they contain MonoBehaviours.

## Installation

### Via Git URL

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL**
3. Enter: `https://github.com/XPromus/unity_orphan_checker.git?path=Assets/OrphanChecker`

### Via Local Install

Copy the contents of this folder into your project's `Packages/` directory:

```
YourProject/
  Packages/
    com.xpromus.orphanchecker/
      package.json
      Editor/
      ...
```
