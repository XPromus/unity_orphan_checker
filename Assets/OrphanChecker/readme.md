# Orphan Checker

Editor tool that scans your project for orphaned assets — assets that are no longer referenced by any scannable file (prefabs, scenes, materials, animators, etc.).

## Usage

Open the tool window via **Tools > Orphan Checker**.

- Click **Check** to scan the project. The window lists orphaned scripts and materials.
- Use the **Show** button next to a material to highlight it in the Project window.
- Use **Clear Selected** to reset all material toggles.

## Installation

### Via Git URL

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL**
3. Enter: `https://github.com/XPromus/unity_orphan_checker.git`

### Via Local Install

Copy the contents of this folder into your project's `Packages/` directory:

```
YourProject/
  Packages/
    com.xpromus.orphanchecker/
      package.json
      Editor/
      Data/
      ...
```
